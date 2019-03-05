using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;

namespace Core
{
    static class UIMessage
    {
        public static MessageBoxResult ShowError(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return result;
        }
    }
}
