using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.bouncycastle.crypto;
using org.bouncycastle.pkcs;

namespace ClientApp.BaseClasses
{
    class Certificate
    {
        private string path = "";
        private string password = "";
        private AsymmetricKeyParameter akp;
        private org.bouncycastle.x509.X509Certificate[] chain;

        public org.bouncycastle.x509.X509Certificate[] Chain
        {
            get { return chain; }
        }

        public AsymmetricKeyParameter Akp
        {
            get { return akp; }
        }

        private void processCert()
        {
            string alias = null;
            PKCS12Store pk12;

            pk12 = new PKCS12Store(new FileStream(this.path, FileMode.Open, FileAccess.Read), this.password.ToCharArray());

            IEnumerator i = pk12.aliases();
            while (i.MoveNext())
            {
                alias = ((string)i.Current);
                if (pk12.isKeyEntry(alias))
                    break;
            }

            this.akp = pk12.getKey(alias).getKey();
            X509CertificateEntry[] ce = pk12.getCertificateChain(alias);
            this.chain = new org.bouncycastle.x509.X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
                chain[k] = ce[k].getCertificate();

        }

        public Certificate(string cpath, string cpassword)
        {
            path = cpath;
            password = cpassword;
            processCert();
        }

    }
}
