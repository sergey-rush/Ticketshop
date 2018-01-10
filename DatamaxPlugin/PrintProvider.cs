using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using DatamaxONeil.Connection;
using DatamaxONeil.Printer;
using DatamaxONeil.Printer.Configuration.DPL;

namespace DatamaxPlugin
{
    internal static class PrintProvider
    {
        public static bool Print(byte[] bytes, string ip, int port)
        {
            bool result = false;
            Connection_TCP conn = null;
            try
            {
                DocumentDPL docDPL = new DocumentDPL();
                ParametersDPL paramDPL = new ParametersDPL();
                docDPL.ColumnOffset = 65;
                docDPL.RowOffset = 0;
                MemoryStream ms = new MemoryStream();
                ms.Write(bytes, 0, bytes.Length);
                Bitmap anImage = new Bitmap(ms);
                anImage.RotateFlip(RotateFlipType.Rotate90FlipX);
                docDPL.WriteImage(anImage, DocumentDPL.ImageType.Other, 0, 0, paramDPL);
                byte[] printData = docDPL.GetDocumentImageData();

                conn = Connection_TCP.CreateClient(ip, port);

                conn.Open();

                uint bytesWritten = 0;
                uint bytesToWrite = 1024;
                uint totalBytes = (uint)printData.Length;
                uint remainingBytes = totalBytes;
                while (bytesWritten < totalBytes)
                {
                    if (remainingBytes < bytesToWrite)
                        bytesToWrite = remainingBytes;
                    conn.Write(printData, (int)bytesWritten, (int)bytesToWrite);
                    bytesWritten += bytesToWrite;
                    remainingBytes = remainingBytes - bytesToWrite;
                }
                conn.WaitForEmptyBuffer(5000);
                conn.Close();
                Thread.Sleep(1000);
                result = true;
            }
            catch
            {
                // ignore
            }
            finally
            {
                if (conn != null) conn.Close();
            }

            return result;
        }

        /// <summary>
        /// STX Kc Configuration Set
        /// This command specifies the Power-up Configuration parameter values for the printer
        /// </summary>
        private static byte[] configCommand = new byte[] { 0x02, 0x4b, 0x43 };

        /// <summary>
        /// Sensor Type STX r, r = Reflective sensor type
        /// This command enables transmissive sensing for bottom bar detection
        /// 
        /// STX r command enables reflective (black mark) sensing for top-of-form detection of rolled
        /// butt-cut, and fan-fold or tag stocks with reflective marks on the underside. This Media
        /// Sensor will detect a minimum mark of 0.1 inches (2.54 mm) between labels (see the
        /// Operator’s Manual for media requirements). The end of the black mark determines the
        /// top of form. Use the STX O command to adjust the print position.
        /// 0x0d, 0x0a - line terminator
        /// </summary>
        private static byte[] sensorTypeCommand = new byte[] { 0x02, 0x72, 0x0d, 0x0a };

        /// <summary>
        /// STX K}Q Quick Media Calibration
        /// This command causes the printer to move media, sample, and then save sensor samples as calibration values.
        /// This calibration function should be performed with media installed in the printer.
        /// </summary>
        private static byte[] quickMediaCalibration = new byte[] { 0x53, 0x54, 0x58, 0x20, 0x4B, 0x7D, 0x51 };

        /// <summary>
        /// STX O Set Start of Print (SOP) Position
        /// This command sets the point to begin printing relative to the top-of-form (the label’s edge as detected by the Media Sensor).
        /// The printer will feed from the top-of-form to the value specified in this command to begin printing.
        /// 0x0d, 0x0a - line terminator
        /// </summary>
        private static byte[] setStartOfPrintPosition = new byte[] { 0x02, 0x4F, 0x30, 0x30, 0x30, 0x30, 0x0d, 0x0a };

        public static void Calibrate(string ip, int port)
        {
            Connection_TCP conn = Connection_TCP.CreateClient(ip, port);
            var ok = conn.Open();
            conn.Write(quickMediaCalibration);
        }

        public static void Setup(string ip, int port)
        {
            Connection_TCP conn = null;
            try
            {
                byte[] bytes = new byte[sensorTypeCommand.Length + setStartOfPrintPosition.Length];
                Buffer.BlockCopy(sensorTypeCommand, 0, bytes, 0, sensorTypeCommand.Length);
                Buffer.BlockCopy(setStartOfPrintPosition, 0, bytes, sensorTypeCommand.Length,
                    setStartOfPrintPosition.Length);

                conn = Connection_TCP.CreateClient(ip, port);
                var ok = conn.Open();
                conn.Write(bytes);
                conn.WaitForEmptyBuffer(5000);
                conn.Close();
                Thread.Sleep(3000);
            }
            catch
            {
                // ignore
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        public static Dictionary<string, object> RunQuery(string clause, string ip, int printPort)
        {
            Connection_TCP conn = null;
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                conn = Connection_TCP.CreateClient(ip, printPort);
                bool gh = conn.Open();
                PrinterInformation_DPL p = new PrinterInformation_DPL();
                p.QueryPrinter(conn, 3000);
                result.Add("General", p);

                MediaLabel_DPL mediaLabel = new MediaLabel_DPL();
                mediaLabel.QueryPrinter(conn, 3000);
                result.Add("MediaLabel", mediaLabel);

                SystemSettings_DPL sysSettings = new SystemSettings_DPL();
                sysSettings.QueryPrinter(conn, 3000);
                result.Add("System", sysSettings);

                PrintSettings_DPL printSettings = new PrintSettings_DPL();
                printSettings.QueryPrinter(conn, 3000);
                result.Add("Print Controls", printSettings);

                SensorCalibration_DPL sensorCalibration = new SensorCalibration_DPL();
                sensorCalibration.QueryPrinter(conn, 3000);
                result.Add("Calibration", sensorCalibration);

                Miscellaneous_DPL misc = new Miscellaneous_DPL();
                misc.QueryPrinter(conn, 3000);
                result.Add("Misc", misc);
                conn.Close();
            }
            catch
            {
                //bool r = false;
            }
            finally
            {
                if (conn != null) conn.Close();
            }

            return result;
        }

        /// <summary>
        /// GoTo: DataMax Mark III: Get Extended Status String
        /// </summary>
        public static Dictionary<ExtendedStatus, bool> GetExtendedStatus(out bool isOk)
        {
            isOk = false;
            byte[] outStream = { 0x01, 0x61, 0x0D };
            var res = new Dictionary<ExtendedStatus, bool> { };
            //using (TcpClient clientSocket = new TcpClient())
            //{
            //    clientSocket.Connect(PrinterIp, PrinterPort);
            //    NetworkStream serverStream = clientSocket.GetStream();

            //    serverStream.Write(outStream, 0, outStream.Length);
            //    serverStream.Flush();
            //    byte[] inStream = new byte[10025];

            //    serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
            //    var returndata = System.Text.Encoding.ASCII.GetString(inStream).ToCharArray();
            //    
            //    for (int i = 1; i <= 22; i++)
            //    {
            //        res.Add((ExtendedStatus)i, returndata[i - 1] != 'N');
            //    }
            //    Log(new string(returndata));
            //    isOk = true;
            //    return res;
            //}
            //Conn.WaitForEmptyBuffer(5000);
            //Thread.Sleep(100);
            //Conn.Write(outStream);
            //Thread.Sleep(100);
            //Conn.WaitForEmptyBuffer(5000);
            //Thread.Sleep(100);
            //var data = Conn.Read();
            //Log(data);
            //isOk = data != "";
            //if (isOk)
            //{
            //    var returndata = data.ToCharArray();

            //    for (int i = 1; i <= 22; i++)
            //    {
            //        res.Add((ExtendedStatus)i, returndata[i - 1] != 'N');
            //    }
            //}
            return res;
        }

    }
}