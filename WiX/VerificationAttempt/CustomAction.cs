using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

namespace WiXCustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult VerifyLog(Session session)
        {
            session.Log("VerifyLog started");
            session["VerifyLog"] = "Started";

            var rtfPath = Path.GetTempPath() + "verificationLog.rtf";

            var sRtfText = @"{\rtf1\ansi\deff0{\colortbl;\red0\green0\blue0;\red255\green0\blue0;}THIS IS NOT DEFAULT TEXT}";
            //var sRtfText = @"{\rtf1\ansi\deff0{\colortbl;\red0\green0\blue0;\red255\green0\blue0;}";
            //sRtfText += @"\cf1";
            //sRtfText += @"This line is the default color\line";
            //sRtfText += @"\cf2";
            //sRtfText += @"This line is red\line";
            //sRtfText += @"\cf1";
            //sRtfText += @"This line is the default color";
            //sRtfText += @"}";

            // no need for saving rtf file. (either way, if Text SourceFile were to be used, the directory has to be defined)
            //session.Log("Attempting to save to rtf file.");
            //try
            //{
            //    using (RichTextBox rtb = new RichTextBox())
            //    {
            //        rtb.Rtf = sRtfText;
            //        rtb.SaveFile(rtfPath, RichTextBoxStreamType.RichText);
            //        session.Log("Attempt success. RTF File saved @ " + rtfPath);
            //    }
            //}
            //catch (Exception e)
            //{
            //    session.Log("Attempt failed");
            //    session.Log(e.ToString());
            //}

            session.Log("Opening the view from control table");
            var view = session.Database.OpenView("SELECT * FROM Control WHERE Dialog_='VerifyDlg' AND Control='VerificationView'");
            session.Log("view.Execute()");
            view.Execute();
            session.Log("view.Fetch()");
            var record = view.Fetch();
            session.Log("Current Text Column: " + record.GetString("Text"));

            session.Log("view.Delete()");
            view.Delete(record);

            // thought actual path needs to be passed, but looking in to orca, the Text is storing the actual rtf string not the format.
            //session.Log("setting the text with rtf path : " + rtfPath);
            //record.SetString("Text", rtfPath);
            session.Log("setting the text with rtf content : " + sRtfText);
            session.Log("Current Text Column Before set string : " + record.GetString("Text"));
            record.SetString("Text", sRtfText);
            session.Log("Current Text Column After set string : " + record.GetString("Text"));

            session.Log("view.Modify");
            session.Log("Current Text Column Before Insert: " + record.GetString("Text"));
            //view.Modify(ViewModifyMode.InsertTemporary, record);
            view.InsertTemporary(record);
            session.Log("Current Text Column After Insert: " + record.GetString("Text"));

            session.Log("Success");

            session["RTText"] = sRtfText;

            // following crashes - wish it worked
            //var view = session.Database.OpenView("UPDATE Control SET Text='" + sRtfText + "' WHERE Dialog_='EndDlg' AND Control='Verification'");
            //view.Execute();

            return ActionResult.Success;
        }
    }
}
