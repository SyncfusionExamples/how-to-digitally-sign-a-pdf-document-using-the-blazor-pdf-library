using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;

namespace BlazorPDF.Data
{
    public class ExportService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ExportService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public MemoryStream CreatePDF()
        {
            FileStream pdfstream = new FileStream(_hostingEnvironment.WebRootPath + "//PDF_Succinctly.pdf", FileMode.Open, FileAccess.Read);
            PdfLoadedDocument document = new PdfLoadedDocument(pdfstream);
            FileStream pfxstream = new FileStream(_hostingEnvironment.WebRootPath + "//DigitalSignatureTest.pfx", FileMode.Open, FileAccess.Read);
            PdfCertificate certificate = new PdfCertificate(pfxstream, "DigitalPass123");
            PdfSignature signature = new PdfSignature(document, document.Pages[0], certificate, "DigitalSignature");
            signature.Bounds = new RectangleF(40, 30, 350, 100);

            FileStream imageStream = new FileStream(_hostingEnvironment.WebRootPath + "//signature.png", FileMode.Open, FileAccess.Read);
            PdfImage image = PdfImage.FromStream(imageStream);

            PdfStandardFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 15);

            signature.Appearance.Normal.Graphics.DrawRectangle(PdfPens.Black, PdfBrushes.White, new RectangleF(50, 0, 300, 100));
            signature.Appearance.Normal.Graphics.DrawImage(image, 0, 0, 100, 100);
            signature.Appearance.Normal.Graphics.DrawString("Digitally Signed by Syncfusion", font, PdfBrushes.Black, 120, 17);
            signature.Appearance.Normal.Graphics.DrawString("Reason: Testing signature", font, PdfBrushes.Black, 120, 39);
            signature.Appearance.Normal.Graphics.DrawString("Location: USA", font, PdfBrushes.Black, 120, 60);

            signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
            signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;

            signature.Certificated = true;
            signature.DocumentPermissions = PdfCertificationFlags.AllowFormFill | PdfCertificationFlags.AllowComments;

            //signature.TimeStampServer = new TimeStampServer(new Uri("http://timestamp.digicert.com/"));

            MemoryStream docstream = new MemoryStream();
            document.Save(docstream);
            document.Close(true);

            PdfLoadedDocument document2 = new PdfLoadedDocument(docstream);

            PdfLoadedSignatureField signatureField = document2.Form.Fields[0] as PdfLoadedSignatureField;
            PdfSignature signature1 = signatureField.Signature;

            CryptographicStandard cryptographicStandard = signature1.Settings.CryptographicStandard;
            DigestAlgorithm digestAlgorithm = signature1.Settings.DigestAlgorithm;

            FileStream pfxstream2 = new FileStream(_hostingEnvironment.WebRootPath + "//TestAgreement.pfx", FileMode.Open, FileAccess.Read);
            PdfCertificate certificate2 = new PdfCertificate(pfxstream2, "Test123");

            PdfSignature signature2 = new PdfSignature(document2, document2.Pages[0], certificate2, "DigitalSignature2");
            //signature2.Settings.CryptographicStandard = cryptographicStandard;
            //signature2.Settings.DigestAlgorithm = digestAlgorithm;

            MemoryStream stream = new MemoryStream();
            document2.Save(stream);
            document2.Close(true);
            stream.Position = 0;
            return stream;

        }
    }
}
