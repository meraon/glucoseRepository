

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Xml.Linq;
using ClosedXML.Excel;
using GlukAppWpf.Models;
using Microsoft.Win32;

namespace GlukAppWpf
{
    public class Serializer
    {
        public static string GlucosesSheet = "Glucoses";
        public static string InsulinsSheet = "Insulins";


        /// <summary>
        /// Exports specified glucose and insulin list to XLWorkbook
        /// </summary>
        /// <param name="glucoses"></param>
        /// <param name="insulins"></param>
        public static XLWorkbook Export(IList<GlucoseItem> glucoses, IList<InsulinItem> insulins)
        {
            var workbook = new XLWorkbook();
            ExportGlucoses(glucoses, workbook);
            ExportInsulins(insulins, workbook);
            return workbook;
        }

        /// <summary>
        /// Imports data from xlsx file to specified glucose and insulin lists. Lists are cleared before insertion.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="glucoses"></param>
        /// <param name="insulins"></param>
        public static void Import(string filePath, IList<GlucoseItem> glucoses, IList<InsulinItem> insulins)
        {
            var workbook = new XLWorkbook(filePath);
            if (workbook.TryGetWorksheet(GlucosesSheet, out var glucosesSheet))
            {
                ImportGlucoses(glucoses, glucosesSheet);
            }

            if (workbook.TryGetWorksheet(InsulinsSheet, out var insulinsSheet))
            {
                ImportInsulins(insulins, insulinsSheet);
            }
        }

        private static void ImportGlucoses(IList<GlucoseItem> glucoses, IXLWorksheet glucosesSheet)
        {
            glucoses.Clear();
            bool firstRow = true;
            foreach (var row in glucosesSheet.RowsUsed())
            {
                if (firstRow)
                {
                    firstRow = false;
                    continue;
                }

                GlucoseItem item = new GlucoseItem(DateTime.Parse(row.Cell(1).Value.ToString()), float.Parse(row.Cell(2).Value.ToString()));
                glucoses.Add(item);
            }
        }

        private static void ImportInsulins(IList<InsulinItem> insulins, IXLWorksheet insulinsSheet)
        {
            insulins.Clear();
            bool firstRow = true;
            foreach (var row in insulinsSheet.RowsUsed())
            {
                if (firstRow)
                {
                    firstRow = false;
                    continue;
                }

                InsulinItem item = new InsulinItem(DateTime.Parse(row.Cell(1).Value.ToString()), 
                    float.Parse(row.Cell(2).Value.ToString()), 
                    bool.Parse(row.Cell(3).Value.ToString()));

                insulins.Add(item);
            }
        }

        private static void ExportGlucoses(IList<GlucoseItem> list, XLWorkbook workbook)
        {
            var worksheet = workbook.AddWorksheet(GlucosesSheet);
            int row = 1;

            worksheet.Cell(row, 1).Value = "Date";
            worksheet.Cell(row, 2).Value = "Value";
            var headerRange = worksheet.Range(row, 1, row, 2);
            headerRange.Style.Fill.BackgroundColor = XLColor.Violet;
            headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            foreach (var item in list)
            {
                row++;
                worksheet.Cell(row, 1).Value = item.Date;
                worksheet.Cell(row, 2).Value = item.Value;
                var rowRange = worksheet.Range(row, 1, row, 2);
                rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }

            var fullRange = worksheet.Range(1, 1, row, 2);
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            fullRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Columns(1, 2).AdjustToContents();
        }

        private static void ExportInsulins(IList<InsulinItem> list, XLWorkbook workbook)
        {
            var worksheet = workbook.AddWorksheet(InsulinsSheet);
            int row = 1;

            worksheet.Cell(row, 1).Value = "Date";
            worksheet.Cell(row, 2).Value = "Value";
            worksheet.Cell(row, 3).Value = "isDayDosage";
            var headerRange = worksheet.Range(row, 1, row, 3);
            headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Fill.BackgroundColor = XLColor.Violet;

            foreach (var item in list)
            {
                row++;
                worksheet.Cell(row, 1).Value = item.Date;
                worksheet.Cell(row, 2).Value = item.Value;
                worksheet.Cell(row, 3).Value = item.IsDayDosage;
                var rowRange = worksheet.Range(row, 1, row, 3);

                rowRange.Style.Fill.BackgroundColor =
            item.IsDayDosage ? XLColor.LemonChiffon : XLColor.PowderBlue;
                rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            }

            var fullRange = worksheet.Range(1, 1, row, 3);
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            fullRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Columns(1, 3).AdjustToContents();
        }
    }
}
