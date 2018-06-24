using MinApp.Actions;
using MinApp.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinApp.Demo.Backend.Controllers
{

    public class DialogController : ApiController
    {

        [Route("dialog/open")]
        public async Task<IActionResult> OpenFileDialog(string filter, bool? allowMultiple)
        {
            string[] result = null;
            await this.RunInSTAThread(() =>
            {
                // Feel free to use System.Windows.Forms, this is end user's machine
                var diagOpen = new OpenFileDialog();

                if (!string.IsNullOrEmpty(filter))
                {
                    diagOpen.Filter = filter;
                }

                if (allowMultiple == true)
                {
                    diagOpen.Multiselect = true;
                }

                // The Windows Dialog UI requires STA thread

                diagOpen.ShowDialog();

                result = diagOpen.FileNames;
            });

            return this.Json(result);
        }

        [Route("dialog/info")]
        public void ShowInfo(string message, string title)
        {
            // Don't use alert, it will show the browser url title
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [Route("dialog/error")]
        public void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async Task RunInSTAThread(Action action)
        {
            var tcs = new TaskCompletionSource<int>();
            var thread = new Thread(() =>
            {
                action();
                tcs.SetResult(0);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            await tcs.Task;
        }

    }

}
