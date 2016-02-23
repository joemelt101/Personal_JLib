using JLib.IO.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLib.IO.Net
{
    public abstract class BrowserCommand
    {
        public event EventHandler<IList<string>> ExecutionComplete = delegate { };

        protected List<string> CommandArguments = new List<string>();

        public BrowserCommand(List<string> commandArguments)
        {
            this.CommandArguments = commandArguments;
        }

        public virtual void Execute(JWebBrowser browser) { }

        protected void RaiseExecutionCompleteEvent(IList<string> returnValues)
        {
            ExecutionComplete(this, returnValues);
        }
    }
}
