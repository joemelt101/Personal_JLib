using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JLib.IO.Net
{
    public class JWebBrowser : WebBrowser
    {
        //TODO: Finish
        public bool CurrentlyExecutingCommands { get; private set; }

        public event EventHandler<List<string>> ExecutionCompleted = delegate { };
        public event EventHandler<float> ProgressMade = delegate { };

        //Used for the creation of asynchronous functionality
        private TaskCompletionSource<bool> _tcsDocumentCompleted = new TaskCompletionSource<bool>();

        public JWebBrowser()
        {
            DocumentCompleted += OnDocumentCompleted;
            Navigating += OnDocumentLoadStarted;
        }

        private void AwaitDocumentCompleted()
        {
            if (ReadyState != WebBrowserReadyState.Complete)
            {
                _tcsDocumentCompleted.Task.Wait();
            }
        }

        private void OnDocumentLoadStarted(object sender, WebBrowserNavigatingEventArgs args)
        {
            Console.WriteLine("Hello");
        }

        private void OnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs eventArgs)
        {
            if (ReadyState != WebBrowserReadyState.Complete)
                return;
            if (_tcsDocumentCompleted.Task.IsCompleted) //ensures the task isn't refired as completed
                return;
            _tcsDocumentCompleted.SetResult(true);
        }

        public new void Navigate(string uri)
        {
            NavigateAsync(uri).Wait();
        }

        public async Task NavigateAsync(string uri)
        {
            //Navigate and force document to load
            base.Navigate(uri);
            
            await _tcsDocumentCompleted.Task;
        }

        public string RetrieveContentsWithIDOrName(string idOrName)
        {
            AwaitDocumentCompleted();
            var elements = Document.All.GetElementsByName(idOrName);

            if (elements.Count > 0)
            {
                return Document.All.GetElementsByName(idOrName)[0].InnerHtml;
            }

            return null;
        }

        public string RetrieveContentsWithClass(string className)
        {
            AwaitDocumentCompleted();
            var elements = Document.All;

            foreach (HtmlElement element in elements)
            {
                if (element.GetAttribute("classname") == className)
                {
                    return element.InnerHtml;
                }
            }

            return null;
        }

        public async Task SubmitFormAsync(int formNum, List<KeyValuePair<string, string>> formArguments)
        {
            AwaitDocumentCompleted();

            //else good to go!

            //grab the pertinent form
            HtmlElement form = Document.Forms[formNum];

            //fill out fields
            foreach (var keyValuePair in formArguments)
            {
                string name = keyValuePair.Key;
                string value = keyValuePair.Value;

                var elements = form.All.GetElementsByName(name);

                if (elements.Count == 0)
                {
                    //couldn't find the specified name!
                    throw new ArgumentException("Error! Could not find the specified element name " + name + " at the URI: " + Url);
                }

                var formControl = elements[0];
                formControl.InnerText = value;
            }

            _tcsDocumentCompleted = new TaskCompletionSource<bool>();
            form.InvokeMember("submit");
            await _tcsDocumentCompleted.Task;
        }
    }
}
