using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TicketShop.Core;
using TicketShop.Shell.ViewModels;

namespace TicketShop.Shell.Views
{
    public partial class BugReportView
    {
        private readonly MainViewModel _mainViewModel;
        public BugReportView()
        {
            _mainViewModel = MainViewModel.Instance;
            DataContext = _mainViewModel;
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Log log = new Log();
            Member member = _mainViewModel.Member;
            log.LogName = member.ToString();
            log.LogText = ConvertRichTextBoxContentsToString(RichTextBoxMessage);
            bool result = Data.Access.SendMessage(log);
            if (result)
            {
                MessageBoxResult dr = MessageBox.Show("Сообщение отправлено. Закрыть окно?", "Отправлено",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.OK)
                {
                    Close();
                }
            }
        }

        private string ConvertRichTextBoxContentsToString(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart,rtb.Document.ContentEnd);
            return textRange.Text;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            Close();
        }
    }
}