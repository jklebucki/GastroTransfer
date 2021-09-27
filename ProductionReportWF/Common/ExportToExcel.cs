using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ProductionReportWF.Common
{
    class ExportToExcel
    {
        public static void dataGridToExcel(string filepath, bool openExisting, DataGridView dg, string sheetName)
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            Int32 test1 = 0, test2 = 0;
            string celVal = "";
            try
            {
                SpreadsheetDocument spreadsheetDocument = null;
                WorkbookPart workbookpart = null;
                WorksheetPart worksheetPart = null;
                Sheets sheets = null;
                SheetData sheetData = null;
                if (!openExisting)
                {
                    try
                    {
                        spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);
                        // Add a WorkbookPart to the document.
                        workbookpart = spreadsheetDocument.AddWorkbookPart();
                        workbookpart.Workbook = new Workbook();

                        // Add a WorksheetPart to the WorkbookPart.
                        worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        // Add Sheets to the Workbook.
                        sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                        // Append a new worksheet and associate it with the workbook.
                        Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
                        sheets.Append(sheet);

                        // Get the sheetData cell table.
                        sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\nError3");
                    }

                }
                else
                {
                    try
                    {
                        spreadsheetDocument = SpreadsheetDocument.Open(filepath, true);
                        // Add a blank WorksheetPart.
                        worksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                        string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart);

                        // Get a unique ID for the new worksheet.
                        uint sheetId = 1;
                        if (sheets.Elements<Sheet>().Count() > 0)
                        {
                            sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        // Append the new worksheet and associate it with the workbook.
                        Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
                        sheets.Append(sheet);
                        sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\nError1");
                    }
                }

                // Add a row to the cell table.
                //Int32 test1 = 0, test2 = 0;
                try
                {
                    var rowCount = Convert.ToUInt32(dg.RowCount);
                    if (dg.AllowUserToAddRows) rowCount -= 1;
                    string cellAdr = "";
                    for (uint i = 0; i <= rowCount; i++)
                    {
                        test1 = Convert.ToInt32(i);
                        Row row;
                        row = new Row() { RowIndex = i + 1 };
                        sheetData.Append(row);
                        // In the new row, find the column location to insert a cell in A1.
                        for (Int32 j = 0; j < dg.ColumnCount; j++)
                        {
                            test2 = j;
                            cellAdr = Convert.ToString(ColumnIndexParse(j + 1)) + (i + 1);
                            Cell refCell = null;
                            foreach (Cell cell in row.Elements<Cell>())
                            {
                                //int ff = string.Compare(cell.CellReference.Value, cellAdr, true);
                                //if (string.Compare(cell.CellReference.Value, cellAdr, true) > 0) // Co za idiota wymyślił ten warunek ?!!!!
                                if (cell.CellReference.Value == cellAdr)
                                {
                                    refCell = cell;
                                    break;
                                }
                            }

                            // Add the cell to the cell table at A1.
                            Cell newCell = new Cell() { CellReference = cellAdr };
                            row.InsertBefore(newCell, refCell);

                            if (i == 0)
                            {
                                newCell.CellValue = new CellValue(dg.Columns[j].HeaderText);
                                newCell.DataType = new EnumValue<CellValues>(CellValues.String);
                            }
                            else if (i > 0)
                            {
                                // Set the cell value.
                                var tmp = dg.Rows[Convert.ToInt32(i - 1)].Cells[j].Value;
                                if (tmp != null)
                                    celVal = tmp.ToString();
                                else
                                    celVal = "";

                                celVal = celVal.Replace("\u001f", "");
                                string dataType = "System.String";
                                if (tmp != null)
                                    dataType = dg.Rows[Convert.ToInt32(i - 1)].Cells[j].Value.GetType().ToString();

                                //MessageBox.Show(dataType + "\n" + dg.Rows[Convert.ToInt32(i - 1)].Cells[j].Value.ToString());
                                if (dataType == "System.String" || dataType == "System.Boolean")
                                {
                                    newCell.CellValue = new CellValue(celVal);
                                    newCell.DataType = new EnumValue<CellValues>(CellValues.String);

                                }
                                else if (dataType == "System.Decimal" || dataType == "System.Int32" || dataType == "System.Double" || dataType == "System.Int16" || dataType == "System.UInt32" || dataType == "System.UInt16")
                                {
                                    newCell.CellValue = new CellValue(celVal.Replace(',', '.'));
                                    newCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                                }
                                else if (dataType == "System.DateTime")
                                {
                                    newCell.CellValue = new CellValue(celVal);
                                    newCell.DataType = new EnumValue<CellValues>(CellValues.String);
                                }
                                else if (dataType == "System.DBNull")
                                {
                                    newCell.CellValue = new CellValue("");
                                    newCell.DataType = new EnumValue<CellValues>(CellValues.String);
                                }
                            }


                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\nError4" + "(" + test1 + ")" + "(" + test2 + ")");
                }
                // Close the document.
                spreadsheetDocument.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nError2", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void listToExcel<T>(string filepath, bool openExisting, List<T> dataList, string sheetName)
        {
            try
            {
                SpreadsheetDocument spreadsheetDocument = null;
                WorkbookPart workbookpart = null;
                WorksheetPart worksheetPart = null;
                Sheets sheets = null;
                SheetData sheetData = null;
                if (!openExisting)
                {
                    try
                    {
                        spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);
                        // Add a WorkbookPart to the document.
                        workbookpart = spreadsheetDocument.AddWorkbookPart();
                        workbookpart.Workbook = new Workbook();

                        // Add a WorksheetPart to the WorkbookPart.
                        worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        // Add Sheets to the Workbook.
                        sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                        // Append a new worksheet and associate it with the workbook.
                        Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
                        sheets.Append(sheet);

                        // Get the sheetData cell table.
                        sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\nError3", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
                else
                {
                    try
                    {
                        spreadsheetDocument = SpreadsheetDocument.Open(filepath, true);
                        // Add a blank WorksheetPart.
                        worksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet(new SheetData());

                        sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                        string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart);

                        // Get a unique ID for the new worksheet.
                        uint sheetId = 1;
                        if (sheets.Elements<Sheet>().Count() > 0)
                        {
                            sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        // Append the new worksheet and associate it with the workbook.
                        Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
                        sheets.Append(sheet);
                        sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\nError1", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                //Add first row as columns headers
                string celVal = "";
                UInt32Value rowCounter = 1;
                Row row = new Row() { RowIndex = 1 };
                sheetData.Append(row);
                int columnCounter = 0;
                foreach (PropertyInfo prop in dataList[0].GetType().GetProperties())
                {
                    var cellAdr = ColumnIndexParse(columnCounter + 1) + rowCounter;
                    Cell refCell = null;
                    foreach (Cell cell in row.Elements<Cell>())
                    {
                        if (cell.CellReference.Value == cellAdr)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                    Cell newCell = new Cell() { CellReference = cellAdr };
                    row.InsertBefore(newCell, refCell);
                    celVal = prop.Name;
                    newCell.DataType = CellValues.String;
                    newCell.CellValue = new CellValue(celVal);
                    columnCounter++;
                }
                rowCounter++;

                //Add data rows
                foreach (var item in dataList)
                {
                    row = new Row() { RowIndex = rowCounter };
                    sheetData.Append(row);
                    columnCounter = 0;

                    foreach (PropertyInfo prop in item.GetType().GetProperties())
                    {
                        var cellAdr = Convert.ToString(ColumnIndexParse(columnCounter + 1)) + rowCounter;
                        Cell refCell = null;
                        foreach (Cell cell in row.Elements<Cell>())
                        {
                            if (cell.CellReference.Value == cellAdr)
                            {
                                refCell = cell;
                                break;
                            }
                        }

                        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        if (type == typeof(decimal) || type == typeof(double) || type == typeof(int) || type == typeof(float) || type == typeof(short))
                        {
                            var val = prop.GetValue(item, null);
                            val = val == null ? 0 : val;
                            Cell newCell = new Cell()
                            {
                                CellReference = cellAdr,
                                DataType = CellValues.Number,
                                CellValue = new CellValue(val.ToString().Replace(',', '.'))
                            };
                            row.InsertBefore(newCell, refCell);
                        }
                        else
                        {
                            var val = prop.GetValue(item, null);
                            val = val == null ? "" : val.ToString();
                            Cell newCell = new Cell()
                            {
                                CellReference = cellAdr,
                                DataType = CellValues.String,
                                CellValue = new CellValue(val.ToString())
                            };
                            row.InsertBefore(newCell, refCell);
                        }
                        columnCounter++;
                    }
                    rowCounter++;
                }
                spreadsheetDocument.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nError2", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string ColumnIndexParse(int colIndex)
        {
            string[] table = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            string colName = "";
            double x = 0, y = 0, z = 0;
            int r1 = 0, r2 = 0, r3 = 0;
            x = ((double)colIndex - 26) / 26;
            y = Math.Ceiling(x);
            z = Math.Floor(x);
            r1 = (int)Math.Round((x - z) * 26, 0);
            if (colIndex < 26) r1 = colIndex;
            if (r1 == 0) r1 = 26;
            r2 = (int)Math.Round((((y / 26) - Math.Floor(y / 26)) * 26), 0);
            if (y == 26) r2 = 26;
            r3 = (int)Math.Floor(x / 26);
            //MessageBox.Show("x: " + x + "\ny: " + y + "\nz: " + z + "\nr1: " + r1 + "\nr2: " + r2 + "\nr3: " + r3);
            if (colIndex > 702) colName = table[r3 - 1];
            if (r2 > 0) colName += table[r2 - 1];
            if (r1 > 0) colName += table[r1 - 1];

            return colName;
        }
    }
}
