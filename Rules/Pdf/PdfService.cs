using iTextSharp.text;
using iTextSharp.text.pdf;
using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using System;
using System.IO;
using Common;
using Permission = Rules.Reports.Permission;
using Rectangle = iTextSharp.text.Rectangle;

namespace SecurePipelineScan.Rules.Pdf
{
    public class PdfService
    {
        private string dir;

        public PdfService(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            this.dir = dir;
        }

        public string CreateReport(string projectName, SecurityReport report)
        {
            string scanDate = DateTime.Now.ToString("dd MMM yyyy");

            Document document = new Document(PageSize.A4, 20, 20, 20, 20);

            //MemoryStream
            string filename = Path.Combine(dir, projectName + ".pdf");
            FileStream PDFData = new FileStream(filename, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, PDFData);
            Rectangle pageSize = writer.PageSize;

            // Open the Document for writing
            document.Open();

            //Setting fonts
            Font titleFont = FontFactory.GetFont("Helvetica", 14, Font.BOLD);
            Font projectFont = FontFactory.GetFont("Helvetica", 12, Font.BOLD);
            Font linkFont = FontFactory.GetFont("Helvetica", 10, Font.UNDERLINE, BaseColor.BLUE);

            //Add elements to the document here
            Image logo = Image.GetInstance(Path.Combine("Pdf", "somecompany.png"));
            logo.Alignment = Image.ALIGN_RIGHT | Image.TEXTWRAP;
            logo.IndentationLeft = 9f;
            logo.SpacingAfter = 9f;
            logo.ScalePercent(80);
            document.Add(logo);

            document.Add(new Paragraph("Results DevOps Security Scan\n", titleFont));
            document.Add(new Paragraph($"Project:  {projectName}\n", projectFont));

            Anchor anchorBlueprint = new Anchor("Rabobank DevOps Security Blueprint", linkFont)
            {
                Reference = "https://confluence.dev.somecompany.nl/display/MTTAS/Secure+Pipelines"
            };
            document.Add(anchorBlueprint);
            document.Add(new Paragraph(""));

            Anchor anchor = new Anchor("Azure DevOps Group Permissions", linkFont)
            {
                Reference = "https://confluence.dev.somecompany.nl/display/vsts/Azure+DevOps+Project+group+permissions"
            };
            document.Add(anchor);
            document.Add(new Paragraph("\n\n\n"));

            PdfPTable table = new PdfPTable(11);
            table.DefaultCell.Phrase = new Phrase()
                {Font = FontFactory.GetFont("Arial", 8, Font.NORMAL)};
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.WidthPercentage = 100;

            table.AddCell(headerCell("Namespace", 1));
            table.AddCell(headerCell("Application Group", 2));
            table.AddCell(headerCell("Level", 1));
            table.AddCell(headerCell("Permission (bit)", 4));
            table.AddCell(headerCell("Actual value", 1));
            table.AddCell(headerCell("Should be", 1));
            table.AddCell(headerCell("Is compliant?", 1));
            table.CompleteRow();

            // Global permissions
            table.AddCell(namespaceCell("Global Permissions", 10));
            table.AddCell(namespaceCell(report.IsCompliant.ToString(), 1));
            table.CompleteRow();

            foreach (var item in report.GlobalPermissions)
            {
                table.AddCell("");
                table.AddCell(appGroupCell(item.ApplicationGroupName, 9));
                table.AddCell(appGroupCell(item.IsCompliant.ToString(), 1));
                table.CompleteRow();

                foreach (var permission in item.Permissions)
                {
                    var readableActualPermission = ReadablePermissionGenerator(permission.ActualPermissionId);
                    var readableShouldBePermission = ReadablePermissionGenerator(permission.ShouldBePermissionId);

                    table.AddCell("");
                    table.AddCell("");
                    table.AddCell("");
                    table.AddCell("");
                    table.AddCell(bodyCell($"{permission.Description} ({permission.PermissionBit})", 4));
                    table.AddCell(bodyCell(readableActualPermission));
                    table.AddCell(bodyCell(readableShouldBePermission));
                    table.AddCell(bodyCell(permission.IsCompliant.ToString()));
                    table.CompleteRow();
                }

                table.AddCell(namespaceCell(" ", 11));
                table.CompleteRow();
            }

            document.Add(table);

            document.Close();
            PDFData.Close();

            return filename;
        }

        private static string ReadablePermissionGenerator(PermissionId? permissionId)
        {
            string readableActualPermission;

            switch (permissionId)
            {
                case PermissionId.NotSet:
                    readableActualPermission = "Not Set";
                    break;
                case PermissionId.AllowInherited:
                    readableActualPermission = "Allow (inherited)";
                    break;
                case PermissionId.DenyInherited:
                    readableActualPermission = "Deny (inherited)";
                    break;
                default:
                    readableActualPermission = permissionId.ToString();
                    break;
            }

            return readableActualPermission;
        }

        private PdfPCell bodyCell(string text, int colSpan = 1)
        {
            return CreateCell(text, 7, colspan: colSpan);
        }

        private PdfPCell headerCell(string text, int colSpan = 1)
        {
            return CreateCell(text, 7, Font.BOLD, colspan: colSpan, background: BaseColor.LIGHT_GRAY);
        }

        private PdfPCell namespaceCell(string text, int colSpan = 1)
        {
            return CreateCell(text, 7, Font.BOLDITALIC, colspan: colSpan);
        }

        private PdfPCell appGroupCell(string text, int colSpan = 1)
        {
            return CreateCell(text, 7, Font.BOLD, colspan: colSpan);
        }

        private PdfPCell CreateCell(string content, int fontSize, int fontStyle = Font.NORMAL, int alignment = 0, int border = Rectangle.NO_BORDER, int colspan = 1, BaseColor background = null)
        {
            var bodyFont = FontFactory.GetFont("Arial", fontSize, fontStyle);

            PdfPCell cell = new PdfPCell(new Phrase(content, bodyFont))
            {
                HorizontalAlignment = alignment,
                Colspan = colspan,
                Border = border,
                BackgroundColor = background
            };
            return cell;
        }
    }
}