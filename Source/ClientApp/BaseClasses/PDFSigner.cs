using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientApp.Elements;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;
using iTextSharp.text.pdf;
using org.bouncycastle.crypto;

namespace ClientApp.BaseClasses
{
    public static class PDFSigner
    {
        public static bool PDFSign(string inputfile, string outputfile, STabCard sTabCard)
        {
            Certificate cert;
            try
            {
                //TODO: окно выбора пароля
                cert = new Certificate(SystemSingleton.Configuration.CertificatePath, "123456");
                MetaData MD = new MetaData();
                MD.Author = SystemSingleton.CurrentSession.FullName;
                MD.Title = sTabCard.Card.Task.Number;
                MD.Subject = sTabCard.Card.DocType.Caption;
                MD.Keywords = sTabCard.Card.Task.Commentary;
                MD.Creator = sTabCard.Card.From.FullName;
                MD.Producer = SystemSingleton.Configuration.CompanyName;

                PdfReader reader = new PdfReader(inputfile);
                PdfStamper st = PdfStamper.CreateSignature(reader, new FileStream(outputfile, FileMode.Create, FileAccess.Write), '\0', null, true);

                st.MoreInfo = MD.getMetaData();
                st.XmpMetadata = MD.getStreamedMetaData();
                PdfSignatureAppearance sap = st.SignatureAppearance;

                sap.SetCrypto(cert.Akp, cert.Chain, null, PdfSignatureAppearance.WINCER_SIGNED);
                sap.Reason = "Completition";
                sap.Contact = MD.Producer;
                sap.Location = SystemSingleton.Configuration.CompanyLocation;
                if (SystemSingleton.Configuration.SignVisible)
                    sap.SetVisibleSignature(new iTextSharp.text.Rectangle(100, 100, 250, 150), 1, null);
                st.Close();
                return true;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendDialogBox(
                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CantSaveFile"),
                    "File Error"
                );
                return false;
            }
        }
    }
}
