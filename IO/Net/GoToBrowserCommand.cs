using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLib.IO.Net
{
    public class GoToBrowserCommand : BrowserCommand
    {
        public GoToBrowserCommand(List<string> commandArguments)
            : base(commandArguments)
        { }

        public override void Execute(JWebBrowser browser)
        {
            //grab the arguments and validate them
            if (CommandArguments.Count == 0)
            {
                throw new ArgumentException("Error parsing arguments! Must pass a valid URL as an argument! No arguments were passed...");
            }

            if (CommandArguments.Count > 1)
            {
                throw new ArgumentException("Command Argument Error! You must pass exactly one argument to a 'goto' command.");
            }

            //test if valid URI
            string argument = this.CommandArguments[0];
            Uri parsedURI;

            if (Uri.TryCreate(argument, UriKind.Absolute, out parsedURI) == false)
            {
                throw new ArgumentException("Command Argument Error! The URI argument '" + argument + "' is not a valid URI");
            }

            //now go to the webpage
            browser.Navigate(parsedURI.AbsoluteUri);
            base.RaiseExecutionCompleteEvent(null);
        }
    }
}
