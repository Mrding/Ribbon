using System;
using System.Windows.Input;
using Luna.Common.Constants;
using Luna.Core.Extensions;

namespace Luna.Statistic.Presenters
{
   public partial class StaffingChartPresenter
    {

       private void OnExportSvcLevelDataExecuted(object sender, ExecutedRoutedEventArgs e)
       {
           var originalPath = Environment.CurrentDirectory;

           string name =
               _schedule.Name.Replace(@"\", " ").Replace(@"/", " ").Replace(@":", " ").Replace(@"?", " ").Replace(
                   @"'", " ").Replace(@"<", " ").Replace(@">", " ").Replace(@"|", " ");
           var dlg = new Microsoft.Win32.SaveFileDialog
           {
               FileName = string.Format("{0} Assigned SL Report", name),
               DefaultExt = ".txt",
               Filter = "Txt documents (.txt)|*.txt",
               CheckPathExists = true,
               OverwritePrompt = true,
               AddExtension = true
           };


           if (dlg.ShowDialog() == true)
           {
               //  Save document
               string[,] strings = null;
               int dayColumns = 0;
               foreach (var item in _serviceQueueContainer.Values)
               {
                   dayColumns = (item.AssignedServiceLevel.Length / 96) - Global.TailDayAmount + 2;

                   strings = new string[98, dayColumns];

                   var day = DateTime.MinValue;
                   for (var i = 0; i < 96; i++)
                   {
                       strings[i + 1, 0] = string.Format("{0}~{1}", day.AddMinutes(15 * i).ToString("HH:mm"), day.AddMinutes(15 * (i + 1)).ToString("HH:mm"));
                   }
                   strings[97, 0] = "Average";

                   for (var i = 0; i < dayColumns - 1; i++)
                   {
                       var loopingStartIndex = i * 96;
                       var columnIndex = i + 1;

                       strings[0, columnIndex] = string.Format("{0:yyyy/M/dd}", _schedule.Start.AddDays(i));

                       for (var j = 0; j < 96; j++)
                       {
                           strings[j + 1, columnIndex] =
                               Convert.ToInt32(item.AssignedServiceLevel[j + loopingStartIndex] * 100).ToString();
                       }
                       //average
                       strings[97, columnIndex] =
                           (Convert.ToInt32(item.DailyAssignedServiceLevel[i] * 100)).ToString();
                   }
               }
               string str = null;
               for (int i = 0; i < 98; i++)
               {
                   for (int j = 0; j < dayColumns - 1; j++)
                   {
                       str += strings[i, j];
                       if (j < dayColumns - 2)
                           str += ",";
                   }
                   str += "\n";
               }

               System.IO.FileStream fs = System.IO.File.Create(dlg.FileName);
               byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
               fs.Write(bytes, 0, bytes.Count());

               fs.Close();

               GC.Collect();
               GC.WaitForPendingFinalizers();
           }
           Environment.CurrentDirectory = originalPath;
       }
    }
}
