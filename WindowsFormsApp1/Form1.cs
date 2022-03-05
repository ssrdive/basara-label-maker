using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Windows.Controls;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public const int IUsb = 2;
        private static readonly HttpClient client = new HttpClient();
        public Form1(String token)
        {
            InitializeComponent();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            loadItemMaster();
        }

        private string GetStatusMsg(int nStatus)
        {
            string errMsg = "";
            switch ((SLCS_ERROR_CODE)nStatus)
            {
                case SLCS_ERROR_CODE.ERR_CODE_NO_ERROR: errMsg = "No Error"; break;
                case SLCS_ERROR_CODE.ERR_CODE_NO_PAPER: errMsg = "Paper Empty"; break;
                case SLCS_ERROR_CODE.ERR_CODE_COVER_OPEN: errMsg = "Cover Open"; break;
                case SLCS_ERROR_CODE.ERR_CODE_CUTTER_JAM: errMsg = "Cutter jammed"; break;
                case SLCS_ERROR_CODE.ERR_CODE_TPH_OVER_HEAT: errMsg = "TPH overheat"; break;
                case SLCS_ERROR_CODE.ERR_CODE_AUTO_SENSING: errMsg = "Gap detection Error (Auto-sensing failure)"; break;
                case SLCS_ERROR_CODE.ERR_CODE_NO_RIBBON: errMsg = "Ribbon End"; break;
                case SLCS_ERROR_CODE.ERR_CODE_BOARD_OVER_HEAT: errMsg = "Board overheat"; break;
                case SLCS_ERROR_CODE.ERR_CODE_MOTOR_OVER_HEAT: errMsg = "Motor overheat"; break;
                case SLCS_ERROR_CODE.ERR_CODE_WAIT_LABEL_TAKEN: errMsg = "Waiting for the label to be taken"; break;
                case SLCS_ERROR_CODE.ERR_CODE_CONNECT: errMsg = "Port open error"; break;
                case SLCS_ERROR_CODE.ERR_CODE_GETNAME: errMsg = "Unknown (or Not supported) printer name"; break;
                case SLCS_ERROR_CODE.ERR_CODE_OFFLINE: errMsg = "Offline (The printer is in an error status)"; break;
                default: errMsg = "Unknown error"; break;
            }
            return errMsg;
        }

        private bool ConnectPrinter()
        {
            string strPort = "";
            int nInterface = IUsb;
            int nBaudrate = 115200, nDatabits = 8, nParity = 0, nStopbits = 0;
            int nStatus = (int)SLCS_ERROR_CODE.ERR_CODE_NO_ERROR;

            nStatus = BXLLApi.ConnectPrinterEx(nInterface, strPort, nBaudrate, nDatabits, nParity, nStopbits);

            if (nStatus != (int)SLCS_ERROR_CODE.ERR_CODE_NO_ERROR)
            {
                BXLLApi.DisconnectPrinter();
                MessageBox.Show(GetStatusMsg(nStatus));
                return false;
            }
            return true;
        }

        private void SendPrinterSettingCommand()
        {
            // 203 DPI : 1mm is about 7.99 dots
            // 300 DPI : 1mm is about 11.81 dots
            // 600 DPI : 1mm is about 23.62 dots
            int dotsPer1mm = (int)Math.Round((float)BXLLApi.GetPrinterDPI() / 25.4f);
            int nPaper_Width = Convert.ToInt32(double.Parse("75") * dotsPer1mm);
            int nPaper_Height = Convert.ToInt32(double.Parse("50") * dotsPer1mm);
            int nMarginX = Convert.ToInt32(double.Parse("0") * dotsPer1mm);
            int nMarginY = Convert.ToInt32(double.Parse("0") * dotsPer1mm);
            int nSpeed = (int)SLCS_PRINT_SPEED.PRINTER_SETTING_SPEED;
            int nDensity = Convert.ToInt32("14");
            int nOrientation = (int)SLCS_ORIENTATION.TOP2BOTTOM;

            int nSensorType = (int)SLCS_MEDIA_TYPE.GAP;

            //	Clear Buffer of Printer
            BXLLApi.ClearBuffer();

            //	Set Label and Printer
            //BXLLApi.SetConfigOfPrinter(BXLLApi.SPEED_50, 17, BXLLApi.TOP, false, 0, true);
            BXLLApi.SetConfigOfPrinter(nSpeed, nDensity, nOrientation, false, 1, true);

            // Select international character set and code table.To
            BXLLApi.SetCharacterset((int)SLCS_INTERNATIONAL_CHARSET.ICS_USA, (int)SLCS_CODEPAGE.FCP_CP1252);

            /* 
                1 Inch : 25.4mm
                1 mm   :  7.99 Dots (XT5-40, SLP-TX400, SLP-DX420, SLP-DX220, SLP-DL410, SLP-T400, SLP-D420, SLP-D220, SRP-770/770II/770III)
                1 mm   :  7.99 Dots (SPP-L310, SPP-L410, SPP-L3000, SPP-L4000) 
                1 mm   :  7.99 Dots (XD3-40d, XD3-40t, XD5-40d, XD5-40t, XD5-40LCT)
                1 mm   : 11.81 Dots (XT5-43, SLP-TX403, SLP-DX423, SLP-DX223, SLP-DL413, SLP-T403, SLP-D423, SLP-D223)
                1 mm   : 11.81 Dots (XD5-43d, XD5-43t, XD5-43LCT)
                1 mm   : 23.62 Dots (XT5-46)
            */

            BXLLApi.SetPaper(nMarginX, nMarginY, nPaper_Width, nPaper_Height, nSensorType, 0, 2 * dotsPer1mm);

            // Direct thermal
            BXLLApi.PrintDirect("STd", true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var item = itemsList.SelectedItem as Item;
            if (!ConnectPrinter())
                return;

            int multiplier = 1;
            // 203 DPI : 1mm is about 7.99 dots
            // 300 DPI : 1mm is about 11.81 dots
            // 600 DPI : 1mm is about 23.62 dots
            int resolution = BXLLApi.GetPrinterDPI();
            int dotsPer1mm = (int)Math.Round((float)resolution / 25.4f);
            if (resolution >= 600)
                multiplier = 3;

            SendPrinterSettingCommand();

            int ptimes = Int32.Parse(printQty.Text);

            for (int i = 0; i < ptimes; i++)
            {
                // Prints string using TrueFont
                //  P1 : Horizontal position (X) [dot]
                //  P2 : Vertical position (Y) [dot]
                //  P3 : Font Name
                //  P4 : Font Size
                //  P5 : Rotation : (0 : 0 degree , 1 : 90 degree, 2 : 180 degree, 3 : 270 degree)
                //  P6 : Italic
                //  P7 : Bold
                //  P8 : Underline
                //  P9 : RLE (Run-length encoding)
                //BXLLApi.PrintTrueFontLib(2 * dotsPer1mm, 5 * dotsPer1mm, "Arial", 14, 0, true, true, false, "Sample Label-1", false);

                BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 5 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_16X25, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "FARMGEAR (PRIVATE) LIMITED");
                // BXLLApi.PrintTrueFont(2 * dotsPer1mm, 5 * dotsPer1mm, "Arial", 14, 0, true, true, false, "Sample Label-1", false);

                //	Draw Lines
                BXLLApi.PrintBlock(1 * dotsPer1mm, 10 * dotsPer1mm, 71 * dotsPer1mm, 11 * dotsPer1mm, (int)SLCS_BLOCK_OPTION.LINE_OVER_WRITING, 0);

                //Print string using Vector Font
                //  P1 : Horizontal position (X) [dot]
                //  P2 : Vertical position (Y) [dot]
                //  P3 : Font selection
                //        U: ASCII (1Byte code)
                //        K: KS5601 (2Byte code)
                //        B: BIG5 (2Byte code)
                //        G: GB2312 (2Byte code)
                //        J: Shift-JIS (2Byte code)
                // P4  : Font width (W)[dot]
                // P5  : Font height (H)[dot]
                // P6  : Right-side character spacing [dot], Plus (+)/Minus (-) option can be used. Ex) 5, +3, -10	
                // P7  : Bold
                // P8  : Reverse printing
                // P9  : Text style  (N : Normal, I : Italic)
                // P10 : Rotation (0 ~ 3)
                // P11 : Text Alignment
                //        L: Left
                //        R: Right
                //        C: Center
                // P12 : Text string write direction (0 : left to right, 1 : right to left)
                // P13 : data to print
                // ※ : Third parameter, 'ASCII' must be set if Bixolon printer is SLP-T400, SLP-T403, SRP-770 and SRP-770II.
                //BXLLApi.PrintVectorFont(22, 65, "U", 34, 34, "0", false, false, false, (int)SLCS_ROTATION.ROTATE_0, SLCS_FONT_ALIGNMENT.LEFTALIGN.ToString(), (int)SLCS_FONT_DIRECTION.LEFTTORIGHT, "Sample Label-2");

                BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 13 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "Part No");
                BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 16 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_19X30, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, item.item_id);
                BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 22 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "Part Name");
                if(item.name.Length > 25)
                {
                    BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 25 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, item.name);
                } else
                {
                    BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 25 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_19X30, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, item.name);
                }
                // BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 31 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "Secondary No");
                // if (item.foreign_id.Length > 17)
                // {
                    // BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 34 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, item.foreign_id);
                // }
                // else
                // {
                    // BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 34 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_19X30, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, item.foreign_id);
                // }
                BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 42 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "No 67/A, Sirisangabo Place,");
                BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 45 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "Polonnaruwa (+94 27 222 7788)");
                //BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 21 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_16X25, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "7th FL, MiraeAsset Venture Tower,");
                //BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 24 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_16X25, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "685, Sampyeong-dong, Bundang-gu,");
                //BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 27 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_16X25, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "Seongnam-si, Gyeonggi-do,");
                //BXLLApi.PrintDeviceFont(2 * dotsPer1mm, 30 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_16X25, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "463-400, KOREA");

                //BXLLApi.PrintDeviceFont(3 * dotsPer1mm, 36 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "POSTAL CODE");
                //BXLLApi.Print1DBarcode(3 * dotsPer1mm, 40 * dotsPer1mm, (int)SLCS_BARCODE.CODE39, 4 * multiplier, 6 * multiplier, 48 * multiplier, (int)SLCS_ROTATION.ROTATE_0, (int)SLCS_HRI.HRI_NOT_PRINT, "1234567890");

                //BXLLApi.PrintDeviceFont(3 * dotsPer1mm, 50 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "AWB:");
                //BXLLApi.Print1DBarcode(3 * dotsPer1mm, 53 * dotsPer1mm, (int)SLCS_BARCODE.CODE93, 4 * multiplier, 8 * multiplier, 90 * multiplier, (int)SLCS_ROTATION.ROTATE_0, (int)SLCS_HRI.HRI_NOT_PRINT, "8741493121");

                //BXLLApi.PrintDeviceFont(4 * dotsPer1mm, 69 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "WEIGHT:");
                //BXLLApi.PrintDeviceFont(29 * dotsPer1mm, 69 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "DELIVERY NO:");
                //BXLLApi.PrintDeviceFont(53 * dotsPer1mm, 69 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_12X20, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, false, "DESTINATION");

                //BXLLApi.PrintDeviceFont(3 * dotsPer1mm, 73 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_32X50, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, true, "30Kg");
                //BXLLApi.PrintDeviceFont(26 * dotsPer1mm, 73 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_32X50, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, true, "425518");
                //BXLLApi.PrintDeviceFont(55 * dotsPer1mm, 73 * dotsPer1mm, (int)SLCS_DEVICE_FONT.ENG_32X50, multiplier, multiplier, (int)SLCS_ROTATION.ROTATE_0, true, "ICN");

                // Prints a DATAMATRIX
                int xString = (35 * dotsPer1mm);
                int yString = (83 * dotsPer1mm);
                //string DataMatrix_data = "BIXOLON Label Printer, This is for test.";
                //BXLLApi.PrintDataMatrix(xString, yString, (int)SLCS_DATAMATRIX_SIZE.DATAMATRIX_SIZE_4, false, (int)SLCS_ROTATION.ROTATE_0, DataMatrix_data);

                // Prints a QRCode
                //  P1 : Horizontal position (X) [dot]
                //  P2 : Vertical position (Y) [dot]
                //  P3 : MODEL selection (1, 2)
                //  P4 : ECC Level (1 ~ 4)
                //  P5 : Size of QRCode (1 ~ 9)
                //  P6 : Rotation (0 ~ 3)
                //  P7 : data to print
                string QRCode_data = item.item_id;
                BXLLApi.PrintQRCode(53 * dotsPer1mm, 30 * dotsPer1mm, (int)SLCS_QRCODE_MODEL.QRMODEL_2, (int)SLCS_QRCODE_ECC_LEVEL.QRECCLEVEL_M, (int)SLCS_QRCODE_SIZE.QRSIZE_7, (int)SLCS_ROTATION.ROTATE_0, QRCode_data);

                // Prints a PDF417
                //  P1 : Horizontal position (X) [dot]
                //  P2 : Vertical position (Y) [dot]
                //  P3 : Maximum Row Count : 3 ~ 90
                //  P4 : Maximum Column Count : 1 ~ 90
                //  P5 : Error Correction Level
                //  P6 : Data compression method
                //  P7 : HRI
                //  P8 : Barcode Origin Point
                //  P9 : Module Width : 2 ~ 9
                //  P10 : Module Height : 4 ~ 99
                //  P11 : Rotation (0 ~ 3)
                //  P12 : data to print
                //xString = (2 * dotsPer1mm);
                //yString = (97 * dotsPer1mm);
                //string PDF417_data = "BIXOLON Label Printer, This is for test.";
                //BXLLApi.PrintPDF417(xString, yString, 8, 8, 0, 0, false, 1, 3 * multiplier, 14 * multiplier, (int)SLCS_ROTATION.ROTATE_0, PDF417_data);

                // Draw BOX (Fill color is black)
                //BXLLApi.PrintBlock(1 * dotsPer1mm, 80 * dotsPer1mm, 71 * dotsPer1mm, 112 * dotsPer1mm, (int)SLCS_BLOCK_OPTION.BOX, 4);
                //BXLLApi.PrintCircle(10, 1055, 3, 2);

                // Print Image
                //BXLLApi.PrintImageLib(1 * dotsPer1mm, 112 * dotsPer1mm, "BIXOLON.bmp", (int)SLCS_DITHER_OPTION.DITHER_1, false);

                //	Print Command
                BXLLApi.Prints(1, 1);
            }

            // Disconnect printer
            BXLLApi.DisconnectPrinter();
        }

        private async void loadItemMaster()
        {
            btnLoadItems.Enabled = false;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var response = await client.GetAsync("https://farmgear.app/api/item/all");
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                Item[] items = js.Deserialize<Item[]>(responseString);

                itemsList.Items.Clear();

                for (int i = 0; i < items.Length; i++)
                {
                    itemsList.Items.Add(items[i]);
                }

                itemsList.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Session expired. Please restart the application");
            }
            btnLoadItems.Enabled = true;
        }

        private async void searchItem()
        {
            searchBtn.Enabled = false;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var response = await client.GetAsync("https://farmgear.app/api/item/search?search=" + Uri.EscapeUriString(searchText.Text));
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                Item[] items = js.Deserialize<Item[]>(responseString);

                itemsList.Items.Clear();

                for (int i = 0; i < items.Length; i++)
                {
                    itemsList.Items.Add(items[i]);
                }

                itemsList.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Session expired. Please restart the application");
            }
            searchBtn.Enabled = true;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            loadItemMaster();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void printQty_TextChanged(object sender, EventArgs e)
        {

        }

        private void printQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            searchItem();
        }
    }
}
