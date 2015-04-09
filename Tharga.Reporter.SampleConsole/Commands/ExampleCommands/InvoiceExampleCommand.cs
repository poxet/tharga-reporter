using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.SampleConsole.Commands.PdfCommands;
using Tharga.Toolkit.Console.Command.Base;
using Font = Tharga.Reporter.Engine.Entity.Font;
using Image = Tharga.Reporter.Engine.Entity.Element.Image;

namespace Tharga.Reporter.SampleConsole.Commands.ExampleCommands
{
    public class InvoiceExampleCommand : ActionCommandBase
    {
        public static UnitValue SumTop = UnitValue.Parse("15cm");
        private static Font _tinyTitle;
        protected Font _dataFont = new Font { Size = 12, FontName = "Times", Bold = true };

        public InvoiceExampleCommand() 
            : base("invoice", "Creates an example invoice")
        {
        }

        public async override Task<bool> InvokeAsync(string paramList)
        {
            //TODO: Put code that creates an example invoice here
            var sections = GetSections();
            var template = new Template(sections.First());
            foreach (var section in sections.TakeAllButFirst())
            {
                template.SectionList.Add(section);
            }

            var documentProperties = new DocumentProperties();
            var sampleData = new DocumentData();

            await PdfCommand.RenderPdfAsync(template, documentProperties, sampleData, false);

            //TODO: Get the xml
            //var xme = template.ToXml();

            return true;
        }

        public static Section GetSection()
        {
            var section = new Section
            {
                Margin = new UnitRectangle
                {
                    Left = UnitValue.Parse("1cm"),
                    Right = UnitValue.Parse("15mm"),
                    Top = UnitValue.Parse("5mm"),
                    Bottom = UnitValue.Parse("0.5cm")
                },
                DefaultFont = new Font
                {
                    FontName = "Times",
                    Size = 12,
                }
            };
            return section;
        }

        public void ApplyHeader(Section section)
        {
            //var settingRepository = new SettingSqlRepository(_databaseHelper);
            //var bgImage = settingRepository.GetSetting("BackgroundImageUrl");
            var bgImage = "http://www.thargelion.se/Images/Logotype/Thargelion-White-Full.png";
            section.Header.Height = UnitValue.Parse("6.5cm");

            var image = new Image
            {
                Source = bgImage,
                Height = UnitValue.Parse("4.2cm"),
                Top = UnitValue.Parse("0cm"),
                IsBackground = true
            };

            section.Header.ElementList.Add(image);
            section.Header.ElementList.Add(new Text
            {
                Value = "Sida {PageNumber} ({TotalPages})",
                TextAlignment = TextBase.Alignment.Right,
                Width = "100%",
                Visibility = PageVisibility.WhenMultiplePages

            });
        }

        public static void ApplyFooter(Color backLineColor, Section section)
        {
            var titleFont = new Font { Color = backLineColor, Size = 8, FontName = "Times" };

            const string top = "3px";
            section.Footer.Height = UnitValue.Parse("2cm");
            var addressRefPoint = new ReferencePoint
            {
                Left = "1cm",
                Top = UnitValue.Parse(top),
                Stack = ReferencePoint.StackMethod.Vertical
            };
            section.Footer.ElementList.Add(addressRefPoint);
            addressRefPoint.ElementList.Add(new Text { Value = "Postadress", Font = titleFont });
            addressRefPoint.ElementList.Add(new Text { Value = "{LocalCompany.StreetName}" });
            addressRefPoint.ElementList.Add(new Text { Value = "{LocalCompany.PostalCode} {LocalCompany.City}" });

            var hallRefPoint = new ReferencePoint
            {
                Left = UnitValue.Parse("5cm"),
                Top = UnitValue.Parse(top),
                Stack = ReferencePoint.StackMethod.Vertical
            };
            section.Footer.ElementList.Add(hallRefPoint);
            hallRefPoint.ElementList.Add(new Text { Value = "Telefon", Font = titleFont });
            hallRefPoint.ElementList.Add(new Text { Value = "{LocalCompany.Phone}" });
            hallRefPoint.ElementList.Add(new Text { Value = "Fax", Font = titleFont });
            hallRefPoint.ElementList.Add(new Text { Value = "{LocalCompany.Fax}" });

            var tradRefPoint = new ReferencePoint
            {
                Left = UnitValue.Parse("9.5cm"),
                Top = UnitValue.Parse(top),
                Stack = ReferencePoint.StackMethod.Vertical
            };
            section.Footer.ElementList.Add(tradRefPoint);
            tradRefPoint.ElementList.Add(new Text { Value = "{EMail1}" });
            tradRefPoint.ElementList.Add(new Text { Value = "{EMail2}" });
            tradRefPoint.ElementList.Add(new Text { Value = "{EMail3}" });

            var bg = new ReferencePoint
            {
                Left = UnitValue.Parse("17cm"),
                Top = UnitValue.Parse(top),
                Stack = ReferencePoint.StackMethod.Vertical
            };
            section.Footer.ElementList.Add(bg);
            bg.ElementList.Add(new Text { Value = "Bankgiro", Font = titleFont });
            bg.ElementList.Add(new Text { Value = "{Bankgiro}" });
        }

        public static void ApplyAddressField(Section section, bool showReference)
        {
            //Customer
            var companyArea = new ReferencePoint
            {
                Left = UnitValue.Parse("11cm"),
                Top = UnitValue.Parse("3cm"),
                Stack = ReferencePoint.StackMethod.Vertical,
                Name = "CompanyArea"
            };
            section.Header.ElementList.Add(companyArea);
            companyArea.ElementList.Add(new Text { Value = "{Company.Name}" });
            companyArea.ElementList.Add(new Text { Value = "{Company.Address.Note}" });
            companyArea.ElementList.Add(new Text { Value = "{Company.Address.StreetName}" });
            companyArea.ElementList.Add(new Text { Value = "{Company.Address.PostalCode} {Company.Address.City}" });
            companyArea.ElementList.Add(new Text { Value = "{Company.Address.Country}" + System.Environment.NewLine });
            companyArea.ElementList.Add(new Text { Value = "Vagnsaldo" + System.Environment.NewLine + "{Company.CartSum}", HideValue = "Company.CartSum" });

            if (showReference)
            {
                var refArea = new ReferencePoint { Left = "4cm", Top = "3cm", Stack = ReferencePoint.StackMethod.Vertical };
                section.Header.ElementList.Add(refArea);
                refArea.ElementList.Add(new Text { Value = "Vår Referens" + System.Environment.NewLine + "{User.Name}" + System.Environment.NewLine, HideValue = "User.Name" });
                refArea.ElementList.Add(new Text { Value = "Er Referens" + System.Environment.NewLine + "{Company.Address.Representative.Name}", HideValue = "Company.Address.Representative.Name" });
                refArea.ElementList.Add(new Text { Value = "Märke: {Mark}", HideValue = "Mark" });
            }
        }

        internal Section GetSectionBase(Color backLineColor, bool showReference)
        {
            var section = GetSection();

            ApplyHeader(section);
            ApplyAddressField(section, showReference);
            ApplyFooter(backLineColor, section);

            return section;
        }

        public static Font TinyTitle
        {
            get { return _tinyTitle ?? (_tinyTitle = new Font { Size = 6, FontName = "Times" }); }
        }

        internal Section GetNoteBase(Color backLineColor, Color backFieldColor, string noteTypeDescription)
        {
            var section = GetSectionBase(backLineColor, true);

            #region Pane


            //Order summary
            var orderSummaryBv = new ReferencePoint { Left = "1cm", Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryBv);
            orderSummaryBv.ElementList.Add(new Line { Width = "100%", Height = "0", Color = backLineColor });
            orderSummaryBv.ElementList.Add(new Line { Top = "1cm", Width = "100%", Height = "0", Color = backLineColor });
            orderSummaryBv.ElementList.Add(new Line { Top = "0", Left = "0", Width = "0", Height = "1cm", Color = backLineColor });
            orderSummaryBv.ElementList.Add(new Line { Top = "0", Right = "0", Width = "0", Height = "1cm", Color = backLineColor });
            orderSummaryBv.ElementList.Add(new Text { Value = noteTypeDescription + " fortsätter på nästa sida.", Left = "0.5cm", Top = "0.5cm", Visibility = PageVisibility.AllButLast });

            var orderSummaryPe = new ReferencePoint { Left = "11cm", Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryPe);
            orderSummaryPe.ElementList.Add(new Line { Height = "1cm", Width = "0", Color = backLineColor, Visibility = PageVisibility.LastPage });
            var peTitle = new Text { Value = "Summa exkl. moms", Font = TinyTitle, Left = UnitValue.Parse("2px"), Visibility = PageVisibility.LastPage };
            orderSummaryPe.ElementList.Add(peTitle);
            var netSaleTotalPriceText = new Text { Value = "{NetSaleTotalPrice}", Top = UnitValue.Parse("0.5cm"), Width = UnitValue.Parse("2.45cm"), TextAlignment = TextBase.Alignment.Right, Visibility = PageVisibility.LastPage };
            orderSummaryPe.ElementList.Add(netSaleTotalPriceText);

            var orderSummaryVat = new ReferencePoint { Left = "13.5cm", Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryVat);
            orderSummaryVat.ElementList.Add(new Line { Height = "1cm", Width = "0", Color = backLineColor, Visibility = PageVisibility.LastPage });
            var vatTiel = new Text { Value = "Moms", Font = TinyTitle, Left = UnitValue.Parse("2px"), Visibility = PageVisibility.LastPage };
            orderSummaryVat.ElementList.Add(vatTiel);
            var vatSaleTotalPriceText = new Text { Value = "{VatSaleTotalPrice}", Top = UnitValue.Parse("0.5cm"), Width = UnitValue.Parse("2.45cm"), TextAlignment = TextBase.Alignment.Right, Visibility = PageVisibility.LastPage };
            orderSummaryVat.ElementList.Add(vatSaleTotalPriceText);

            var orderSummaryGross = new ReferencePoint { Left = "16cm", Top = SumTop };
            section.Pane.ElementList.Add(orderSummaryGross);
            orderSummaryGross.ElementList.Add(new Line { Height = "1cm", Width = "0", Color = backLineColor, Visibility = PageVisibility.LastPage });
            var grossTitle = new Text { Value = "Att betala", Font = TinyTitle, Left = UnitValue.Parse("2px"), Visibility = PageVisibility.LastPage };
            orderSummaryGross.ElementList.Add(grossTitle);
            var grossSaleTotalPriceText = new Text { Value = "{GrossSaleTotalPrice}", Top = UnitValue.Parse("0.5cm"), Width = "24mm", TextAlignment = TextBase.Alignment.Right, Visibility = PageVisibility.LastPage };
            orderSummaryGross.ElementList.Add(grossSaleTotalPriceText);

            //Extra info
            var titleFont = new Font { Color = backLineColor, Size = 8, FontName = "Times" };
            var vatInfoReference = new ReferencePoint { Left = "1cm", Top = SumTop };
            var vatInfoText = new Text { Value = "Org.nr {LocalCompany.OrgNo}. Innehar F-skattsedel. Vat nr {LocalCompany.VatNo}.", Left = UnitValue.Parse("2px"), Top = UnitValue.Parse("1cm"), Font = titleFont, Visibility = PageVisibility.LastPage };
            vatInfoReference.ElementList.Add(vatInfoText);
            section.Pane.ElementList.Add(vatInfoReference);

            section.Pane.ElementList.Add(new Line { Left = "1cm", Top = "100%" });


            #endregion

            return section;
        }

        private static Table GetInvoiceTable(Color? backLineColor, Color? backFieldColor)
        {
            var orderItemTable = new Table
            {
                Name = "OrderItems",
                Height = SumTop,
                Left = "1cm",

                HeaderFont = new Font { Size = 10, FontName = "Times" },
                HeaderBorderColor = backLineColor ?? Color.Black,
                HeaderBackgroundColor = backFieldColor,

                ContentFont = new Font { Size = 10, FontName = "Times" },
                ContentBorderColor = null, //Color.Aqua,

                GroupSpacing = "5mm",
                GroupFont = new Font { Size = 12, Bold = true, FontName = "Times" },

                SkipLine = new SkipLine
                {
                    Interval = 3,
                    Height = "2mm"
                }
            };
            orderItemTable.AddColumn("{Description}", "Specifikation", widthMode: Table.WidthMode.Spring);
            orderItemTable.AddColumn("{Details}", "Detalj", "2.5cm");
            orderItemTable.AddColumn("{DateAdded}", "Datum", "1cm", Table.WidthMode.Auto, Table.Alignment.Left, string.Empty);
            orderItemTable.AddColumn("{AddedBy}", "Ref", "1cm", Table.WidthMode.Auto, Table.Alignment.Left, string.Empty);
            orderItemTable.AddColumn("{AmountDescription}", "Lådor", "2cm", alignment: Table.Alignment.Right, hideValue: string.Empty);
            orderItemTable.AddColumn("{Count}", "Antal", "2cm", Table.WidthMode.Auto, Table.Alignment.Right);
            orderItemTable.AddColumn("{NetNormalItemPrice}", "Pris", "2.5cm", alignment: Table.Alignment.Right, hideValue: "{NetSaleItemPrice}");
            orderItemTable.AddColumn("{DiscountPercentage}%", "Rabatt", "2.5cm", alignment: Table.Alignment.Right, hideValue: "0%");
            orderItemTable.AddColumn("{NetSaleItemPrice}", "Säljpris", "2cm", alignment: Table.Alignment.Right);
            orderItemTable.AddColumn("{NetSaleTotalPrice}", "Belopp", "2cm", alignment: Table.Alignment.Right);

            return orderItemTable;
        }

        private static Table GetPaymentTable(Color? backLineColor, Color? backFieldColor)
        {
            var paymentTable = new Table
            {
                Name = "Payments",
                Width = "50%",
                Left = "50%",
                HeaderFont = new Font { Size = 10, FontName = "Times" },
                HeaderBorderColor = backLineColor ?? Color.Black,
                HeaderBackgroundColor = backFieldColor,

                ContentFont = new Font { Size = 10, FontName = "Times" },
                ContentBorderColor = null, //Color.Aqua,

            };

            paymentTable.AddColumn("{PaymentMethod}", "Betalsätt", widthMode: Table.WidthMode.Spring);
            paymentTable.AddColumn("{PaymentDate}", "Datum", "2cm");
            paymentTable.AddColumn("{PaymentSum}", "Summa", "2cm", Table.WidthMode.Auto, Table.Alignment.Right, string.Empty);

            return paymentTable;
        }

        protected IEnumerable<Section> GetSections()
        {
            var backFieldColor = Color.White;
            var backLineColor = Color.FromArgb(255, 0, 0, 0);

            //var reportBusinessHelper = new ReportBusinessHelper(_databaseHelper);
            var section = GetNoteBase(backLineColor, backFieldColor, "Fakturan");

            #region Header


            //TODO: Create a reference pount to hang all theese items on
            section.Header.ElementList.Add(new Text { Value = "{NoteType}", Left = "10cm", Top = "0", Font = new Font { Size = 22, FontName = "Times" } });
            section.Header.ElementList.Add(new Line { Left = "10cm", Width = "100%", Top = "22px", Height = "0cm", Color = backLineColor });
            section.Header.ElementList.Add(new Text { Value = "Nr:", Left = "10cm", Top = "24px" });
            section.Header.ElementList.Add(new Text { Value = "{Number}", Left = "12.5cm", Top = "24px", Font = _dataFont });
            section.Header.ElementList.Add(new Text { Value = "Datum:", Left = "10cm", Top = "36px" });
            section.Header.ElementList.Add(new Text { Value = "{Date}", Left = "12.5cm", Top = "36px", Font = _dataFont });
            section.Header.ElementList.Add(new Text { Value = "Kundnr:", Left = "10cm", Top = "54px" });
            section.Header.ElementList.Add(new Text { Value = "{Company.Number}", Left = "12.5cm", Top = "54px", Font = _dataFont });


            #endregion
            #region Pane

            section.Pane.ElementList.Add(GetInvoiceTable(backLineColor, backFieldColor));

            var paymentsReferencePoint = new ReferencePoint { Left = "1cm", Top = "163mm" };
            section.Pane.ElementList.Add(paymentsReferencePoint);

            paymentsReferencePoint.ElementList.Add(GetPaymentTable(backLineColor, backFieldColor));


            //section.Pane.ElementList.Add(new BarCode { Code = "S{Number}", Left = "1cm", Width = "8cm", Top = "17.5cm", Height = "1.5cm", Visibility = PageVisibility.LastPage });
            section.Pane.ElementList.Add(new Text { Top = "19.5cm", Left = "1cm", Value = "Vid betalning efter förfallodagen debiteras 18%", Visibility = PageVisibility.LastPage });
            //section.Pane.ElementList.Add(new Text { Top = "17cm", Left = "10cm", Value = "Vill du automatiskt få denna faktura skickad via mejl varje gång du handlar hos Skalleberg. Skicka då ett meddelande till support@thargelion.se med ditt kundnummer så fixar vi det.", Visibility = PageVisibility.LastPage });
            //section.Pane.ElementList.Add(new Text { Top = "19.5cm", Left = "10cm", Value = "OBS! ENDAST BANKGIRO!", Font = new Font { Bold = true, Size = 12 }, Visibility = PageVisibility.LastPage } );

            var orderSummaryOcr = new ReferencePoint { Top = SumTop, Left = "1cm" };
            section.Pane.ElementList.Add(orderSummaryOcr);
            var ocrTitle = new Text { Value = "OCR", Font = TinyTitle, Visibility = PageVisibility.LastPage };
            orderSummaryOcr.ElementList.Add(ocrTitle);
            var ocrText = new Text { Value = "{OCR}", Top = UnitValue.Parse("0.5cm"), TextAlignment = TextBase.Alignment.Left, Left = UnitValue.Parse("2px"), Visibility = PageVisibility.LastPage };
            orderSummaryOcr.ElementList.Add(ocrText);

            var orderSummaryPaymentDate = new ReferencePoint { Top = SumTop, Left = "8cm" };
            section.Pane.ElementList.Add(orderSummaryPaymentDate);
            orderSummaryPaymentDate.ElementList.Add(new Line { Height = "1cm", Width = "0", Color = backLineColor, Visibility = PageVisibility.LastPage });
            var pdTitle = new Text { Value = "Förfallodatum", Font = TinyTitle, Visibility = PageVisibility.LastPage };
            orderSummaryPaymentDate.ElementList.Add(pdTitle);
            var pdText = new Text { Value = "{ExpirationDate}", Top = "5mm", TextAlignment = TextBase.Alignment.Right, Left = "2px", Width = "2.7cm", Visibility = PageVisibility.LastPage };
            orderSummaryPaymentDate.ElementList.Add(pdText);

            //section.Pane.ElementList.Add(new Text { Left = "10cm", Top = "18cm", Value = "Märke: {Mark}", HideValue = "Mark" });

            //////Signature for the representative
            ////var signRefPoint = new ReferencePoint("1cm", "18cm");
            ////section.Pane.ElementList.Add(signRefPoint);
            ////signRefPoint.ElementList.Add(new Line("0cm", "0cm", "8cm", "0cm", backLineColor, "1px"));
            ////signRefPoint.ElementList.Add(Text.Create("{Representative.Name}", "0", "0cm"));

            //section.Pane.ElementList.Add(GetInvoiceTable(backLineColor, backFieldColor));

            #endregion

            yield return section;
        }
    }
}
