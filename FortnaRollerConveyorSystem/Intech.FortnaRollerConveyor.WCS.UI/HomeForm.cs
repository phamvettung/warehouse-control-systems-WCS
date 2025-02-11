using Intech.FortnaRollerConveyor.Client;
using Intech.FortnaRollerConveyor.Shared.Enums;
using Intech.FortnaRollerConveyor.Shared.Messages;
using Intech.FortnaRollerConveyor.WCS.UI.Configurations;
using Intech.FortnaRollerConveyor.WCS.UI.Models;
using Intech.FortnaRollerConveyor.WCS.UI.UserControls;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zuby.ADGV;
using Container = Intech.FortnaRollerConveyor.WCS.UI.Models.Container;

namespace Intech.FortnaRollerConveyor.WCS.UI
{
    public partial class HomeForm : Form
    {
        #region Configurations
        WesConfig wesConfig = AppConfig.GetWESConfig();
        ScannerConfig scanner01Config = AppConfig.GetScanner01Config();
        ScannerConfig scanner02Config = AppConfig.GetScanner02Config();
        #endregion

        #region Properties
        public static Client.Client WesSocket { get; set; }
        public static Client.Client Scanner01Socket { get; set; }
        public static Client.Client Scanner02Socket { get; set; }
        public static PLC Plc { get; set; } = null;
        public static Grapher grapher { get; set; }
        #endregion

        #region Controls Feilds

        MainConveyor mainConveyor;
        VerticalConveyor verticalConveyor_GTP2_Left;
        VerticalConveyor verticalConveyor_GTP2_Right;
        VerticalConveyor verticalConveyor_GTP1_Left;
        VerticalConveyor verticalConveyor_GTP1_Right;
        HorizoltalConveyor horizoltalConveyor_GTP2;
        HorizoltalConveyor horizoltalConveyor_GTP1;

        public ContainerPanel container0_1;
        public ContainerPanel container1_2;
        public ContainerPanel container2_3;
        public ContainerPanel container3_4;
        public ContainerPanel container4_5;
        public ContainerPanel container5_6;
        public ContainerPanel container7_8;
        public ContainerPanel container8_9;
        public ContainerPanel container9_10;
        public ContainerPanel container10_11;
        public ContainerPanel container11_12;
        public ContainerPanel container2_7;
        public ContainerPanel container8_13;

        private Panel pnSs_01, pnSs_03A, pnSs_03B, pnSs_05, pnSs_07A, pnSs_07B, pnSc_01, pnSs_01sc, pnSc_02, pnSs_02sc;

        List<ContainerPanel>[,] containerPanels = new List<ContainerPanel>[AppConfig.SIZE_OF_CONTAINER, AppConfig.SIZE_OF_CONTAINER];
        public static List<Container> StorehouseContainer = new List<Container>();

        List<Panel> stopperPanels = new List<Panel>();
        List<Panel> pusherPanels = new List<Panel>();
        List<Panel> sensorPanels = new List<Panel>();

        #endregion


        #region Timers
        System.Windows.Forms.Timer tmrDisplayContainer;
        System.Windows.Forms.Timer tmrDisplayDataControl;
        System.Windows.Forms.Timer tmrDisplayNote;
        System.Windows.Forms.Timer tmrRemoveContainer;
        #endregion


        #region Delegates
        public delegate void ShowReady(string message, Color color);
        public static event ShowReady OnShowReady;
        #endregion


        #region Contructors
        public HomeForm()
        {
            InitializeComponent();
            InitializeControl();
            RegisterEvents();
            InitializeConnection();
            InitializeTimer();

        }

        private void InitializeControl()
        {
            int scale = MainConveyor.SCALE;

            mainConveyor = new MainConveyor();
            mainConveyor.Location = new System.Drawing.Point(MainConveyor.START_X, MainConveyor.START_Y);
            this.Controls.Add(mainConveyor);
            mainConveyor.SendToBack();

            verticalConveyor_GTP2_Left = new VerticalConveyor();
            verticalConveyor_GTP2_Left.pnStopperBottom.Visible = false;
            verticalConveyor_GTP2_Left.Location = new System.Drawing.Point((int)(MainConveyor.START_X + MainConveyor.L916), (int)(MainConveyor.START_Y + MainConveyor.L560));
            this.Controls.Add(verticalConveyor_GTP2_Left);
            verticalConveyor_GTP2_Left.SendToBack();

            verticalConveyor_GTP2_Right = new VerticalConveyor();
            verticalConveyor_GTP2_Right.pnStopperTop.Visible = false;
            verticalConveyor_GTP2_Right.Location = new System.Drawing.Point((int)(MainConveyor.START_X + MainConveyor.L916 + MainConveyor.L560 + MainConveyor.L680), (int)(MainConveyor.START_Y + MainConveyor.L560));
            this.Controls.Add(verticalConveyor_GTP2_Right);
            verticalConveyor_GTP2_Right.SendToBack();

            verticalConveyor_GTP1_Left = new VerticalConveyor();
            verticalConveyor_GTP1_Left.pnStopperBottom.Visible = false;
            verticalConveyor_GTP1_Left.Location = new System.Drawing.Point((int)(MainConveyor.START_X + MainConveyor.L916 + MainConveyor.L560 + MainConveyor.L680 + MainConveyor.L560 + MainConveyor.L1000), (int)(MainConveyor.START_Y + MainConveyor.L560));
            this.Controls.Add(verticalConveyor_GTP1_Left);
            verticalConveyor_GTP1_Left.SendToBack();

            verticalConveyor_GTP1_Right = new VerticalConveyor();
            verticalConveyor_GTP1_Right.pnStopperTop.Visible = false;
            verticalConveyor_GTP1_Right.Location = new System.Drawing.Point((int)(MainConveyor.START_X + MainConveyor.L916 + MainConveyor.L560 + MainConveyor.L680 + 2 * MainConveyor.L560 + MainConveyor.L1000 + MainConveyor.L680), (int)(MainConveyor.START_Y + MainConveyor.L560));
            this.Controls.Add(verticalConveyor_GTP1_Right);
            verticalConveyor_GTP1_Right.SendToBack();

            horizoltalConveyor_GTP2 = new HorizoltalConveyor();
            horizoltalConveyor_GTP2.Location = new System.Drawing.Point((int)(MainConveyor.START_X + MainConveyor.L916), (int)(MainConveyor.START_Y + MainConveyor.L560 + MainConveyor.L1200));
            this.Controls.Add(horizoltalConveyor_GTP2);
            horizoltalConveyor_GTP2.SendToBack();

            horizoltalConveyor_GTP1 = new HorizoltalConveyor();
            horizoltalConveyor_GTP1.Location = new System.Drawing.Point((int)(MainConveyor.START_X + MainConveyor.L916 + MainConveyor.L560 + MainConveyor.L680 + MainConveyor.L560 + MainConveyor.L1000), (int)(MainConveyor.START_Y + MainConveyor.L560 + MainConveyor.L1200));
            this.Controls.Add(horizoltalConveyor_GTP1);
            horizoltalConveyor_GTP1.SendToBack();


            for (int i = 0; i < AppConfig.SIZE_OF_CONTAINER; i++)
            {
                for (int j = 0; j < AppConfig.SIZE_OF_CONTAINER; j++)
                {
                    containerPanels[i, j] = new List<ContainerPanel>(AppConfig.SIZE_OF_CONTAINER);
                }
            }
            //Containers
            int step = 0;
            step = 460 / scale + 5;
            container0_1 = new ContainerPanel();
            container0_1.BackColor = Color.Blue;
            container0_1.Location = new Point(MainConveyor.START_X
                + 960 / scale
                + 560 / scale
                + 680 / scale
                + 560 / scale
                + 1000 / scale
                + 560 / scale
                + 680 / scale + 10 + step, MainConveyor.START_Y + 20);
            this.Controls.Add(container0_1);
            containerPanels[0, 1].Add(container0_1);

            container1_2 = new ContainerPanel();
            container1_2.BackColor = Color.Blue;
            container1_2.Location = new Point(MainConveyor.START_X
                + 960 / scale
                + 560 / scale
                + 680 / scale
                + 560 / scale
                + 1000 / scale
                + 560 / scale
                + 680 / scale + 10, MainConveyor.START_Y + 20);
            this.Controls.Add(container1_2);
            containerPanels[1, 2].Add(container1_2);

            container2_3 = new ContainerPanel();
            container2_3.BackColor = Color.Blue;
            container2_3.Location = new Point(MainConveyor.START_X +
                960 / scale +
                560 / scale +
                680 / scale +
                560 / scale +
                1000 / scale +
                560 / scale +
                680 / scale + 10, MainConveyor.START_Y + 1200 / scale + 11);
            this.Controls.Add(container2_3);
            containerPanels[2, 3].Add(container2_3);
            step = 330 / scale + 5;
            for (int i = 0; i < 2; i++)
            {
                ContainerPanel container = new ContainerPanel();
                container.BackColor = Color.Blue;
                container.Location = new Point(MainConveyor.START_X +
                    960 / scale +
                    560 / scale +
                    680 / scale +
                    560 / scale +
                    1000 / scale +
                    560 / scale +
                    680 / scale + 10, MainConveyor.START_Y + 1200 / scale + 11 - step);
                this.Controls.Add(container);
                containerPanels[2, 3].Add(container);
                step += 330 / scale + 5;
            }

            container3_4 = new ContainerPanel();
            container3_4.BackColor = Color.Blue;
            container3_4.Location = new Point(MainConveyor.START_X +
                960 / scale +
                1800 / scale +
                1000 / scale +
                560 / scale + 10, MainConveyor.START_Y + 1200 / scale + 560 / scale + 20);
            this.Controls.Add(container3_4);
            containerPanels[3, 4].Add(container3_4);
            step = 460 / scale + 5;
            for (int i = 0; i < 1; i++)
            {
                ContainerPanel container = new ContainerPanel();
                container.BackColor = Color.Blue;
                container.Location = new Point(MainConveyor.START_X +
                    960 / scale +
                    1800 / scale +
                    1000 / scale +
                    560 / scale + 10 + step, MainConveyor.START_Y + 1200 / scale + 560 / scale + 20);
                this.Controls.Add(container);
                containerPanels[3, 4].Add(container);
                step += 460 / scale + 5;
            }

            container4_5 = new ContainerPanel();
            container4_5.BackColor = Color.Blue;
            container4_5.Location = new Point(MainConveyor.START_X +
                960 / scale +
                1800 / scale +
                1000 / scale + 9, MainConveyor.START_Y + 1200 / scale + 560 / scale + 20);
            this.Controls.Add(container4_5);
            containerPanels[4, 5].Add(container4_5);

            container5_6 = new ContainerPanel();
            container5_6.BackColor = Color.Blue;
            container5_6.Location = new Point(MainConveyor.START_X +
                960 / scale +
                1800 / scale +
                1000 / scale + 9, MainConveyor.START_Y + 560 / scale + 26);
            this.Controls.Add(container5_6);
            containerPanels[5, 6].Add(container5_6);
            step = 330 / scale + 5;
            for (int i = 0; i < 2; i++)
            {
                ContainerPanel container = new ContainerPanel();
                container.BackColor = Color.Blue;
                container.Location = new Point(MainConveyor.START_X +
                    960 / scale +
                    1800 / scale +
                    1000 / scale + 9, MainConveyor.START_Y + 560 / scale + 26 + step);
                this.Controls.Add(container);
                containerPanels[5, 6].Add(container);
                step += 330 / scale + 5;
            }

            container2_7 = new ContainerPanel();
            container2_7.BackColor = Color.Blue;
            container2_7.Location = new Point(MainConveyor.START_X +
                960 / scale +
                560 / scale +
                680 / scale + 10 +
                460 / scale + 5, MainConveyor.START_Y + 20);
            this.Controls.Add(container2_7);
            containerPanels[2, 7].Add(container2_7);
            step = 460 / scale + 5;
            for (int i = 0; i < 3; i++)
            {
                ContainerPanel container = new ContainerPanel();
                container.BackColor = Color.Blue;
                container.Location = new Point(MainConveyor.START_X +
                    960 / scale +
                    560 / scale +
                    680 / scale + 10 + 460 / scale + 5 + step, MainConveyor.START_Y + 20);
                this.Controls.Add(container);
                containerPanels[2, 7].Add(container);
                step += 460 / scale + 5;
            }

            container7_8 = new ContainerPanel();
            container7_8.BackColor = Color.Blue;
            container7_8.Location = new Point(MainConveyor.START_X +
                960 / scale +
                560 / scale +
                680 / scale + 10, MainConveyor.START_Y + 20);
            this.Controls.Add(container7_8);
            containerPanels[7, 8].Add(container7_8);

            container8_9 = new ContainerPanel();
            container8_9.BackColor = Color.Blue;
            container8_9.Location = new Point(MainConveyor.START_X +
                960 / scale +
                560 / scale +
                680 / scale + 10, MainConveyor.START_Y + 1200 / scale + 11);
            this.Controls.Add(container8_9);
            containerPanels[8, 9].Add(container8_9);
            step = 330 / scale + 5;
            for (int i = 0; i < 2; i++)
            {
                ContainerPanel container = new ContainerPanel();
                container.BackColor = Color.Blue;
                container.Location = new Point(MainConveyor.START_X + 960 / scale + 560 / scale + 680 / scale + 10, MainConveyor.START_Y + 1200 / scale + 11 - step);
                this.Controls.Add(container);
                containerPanels[8, 9].Add(container);
                step += 330 / scale + 5;
            }


            container9_10 = new ContainerPanel();
            container9_10.BackColor = Color.Blue;
            container9_10.Location = new Point(MainConveyor.START_X + 960 / scale + 560 / scale + 10, MainConveyor.START_Y + 1200 / scale + 560 / scale + 20);
            this.Controls.Add(container9_10);
            containerPanels[9, 10].Add(container9_10);
            step = 460 / scale + 5;
            for (int i = 0; i < 1; i++)
            {
                ContainerPanel container = new ContainerPanel();
                container.BackColor = Color.Blue;
                container.Location = new Point(MainConveyor.START_X + 960 / scale + 560 / scale + 10 + step, MainConveyor.START_Y + 1200 / scale + 560 / scale + 20);
                this.Controls.Add(container);
                containerPanels[9, 10].Add(container);
                step += 460 / scale + 5;
            }

            container10_11 = new ContainerPanel();
            container10_11.BackColor = Color.Blue;
            container10_11.Location = new Point(MainConveyor.START_X + 960 / scale + 9, MainConveyor.START_Y + 1200 / scale + 560 / scale + 20);
            this.Controls.Add(container10_11);
            containerPanels[10, 11].Add(container10_11);

            container11_12 = new ContainerPanel();
            container11_12.BackColor = Color.Blue;
            container11_12.Location = new Point(MainConveyor.START_X + 960 / scale + 9, MainConveyor.START_Y + 560 / scale + 26);
            this.Controls.Add(container11_12);
            containerPanels[11, 12].Add(container11_12);
            step = 330 / scale + 5;
            for (int i = 0; i < 2; i++)
            {
                ContainerPanel container = new ContainerPanel();
                container.BackColor = Color.Blue;
                container.Location = new Point(MainConveyor.START_X + 960 / scale + 9, MainConveyor.START_Y + 560 / scale + 26 + step);
                this.Controls.Add(container);
                containerPanels[11, 12].Add(container);
                step += 330 / scale + 5;
            }

            container8_13 = new ContainerPanel();
            container8_13.BackColor = Color.Blue;
            container8_13.Location = new Point(MainConveyor.START_X + 25, MainConveyor.START_Y + 20);
            this.Controls.Add(container8_13);
            containerPanels[8, 13].Add(container8_13);
            step = 460 / scale + 5;
            for (int i = 0; i < 3; i++)
            {
                ContainerPanel container = new ContainerPanel();
                container.BackColor = Color.Blue;
                container.Location = new Point(MainConveyor.START_X + 25 + step, MainConveyor.START_Y + 20);
                this.Controls.Add(container);
                containerPanels[8, 13].Add(container);
                step += 460 / scale + 5;
            }

            for (int i = 0; i < AppConfig.SIZE_OF_CONTAINER; i++)
            {
                for (int j = 0; j < AppConfig.SIZE_OF_CONTAINER; j++)
                {
                    for (int k = 0; k < containerPanels[i, j].Count; k++)
                    {
                        containerPanels[i, j][k].BringToFront();
                        containerPanels[i, j][k].Visible = false;
                    }
                }
            }

            stopperPanels.Add(pnStopper01);
            stopperPanels.Add(pnStopper02);
            stopperPanels.Add(pnStopper03);
            stopperPanels.Add(pnStopper04);
            stopperPanels.Add(pnStopper05);
            stopperPanels.Add(pnStopper06);
            stopperPanels.Add(pnStopper07);
            stopperPanels.Add(pnStopper08);

            pusherPanels.Add(pnPusher01);
            pusherPanels.Add(pnPusher02);
            pusherPanels.Add(pnPusher03);
            pusherPanels.Add(pnPusher04);
            pusherPanels.Add(pnPusher05);
            pusherPanels.Add(pnPusher06);
            pusherPanels.Add(pnPusher07);
            pusherPanels.Add(pnPusher08);

            sensorPanels.Add(pnSensor01);
            sensorPanels.Add(pnSensor02);
            sensorPanels.Add(pnSensor03);
            sensorPanels.Add(pnSensor04);
            sensorPanels.Add(pnSensor05);
            sensorPanels.Add(pnSensor06);
            sensorPanels.Add(pnSensor07);
            sensorPanels.Add(pnSensor08);

            //Initialize sensors
            pnSs_01 = new Panel();
            pnSs_01.BorderStyle = BorderStyle.FixedSingle;
            pnSs_01.Size = new Size(10, 10);
            pnSs_01.BackColor = Color.LightGray;
            pnSs_01.Location = new Point((
                MainConveyor.START_X +
                MainConveyor.L916 +
                MainConveyor.L560 +
                MainConveyor.L680 +
                MainConveyor.L560 +
                MainConveyor.L1000 +
                MainConveyor.L560 +
                MainConveyor.L680) + 10,
                MainConveyor.START_Y - 5);
            this.Controls.Add(pnSs_01);
            pnSs_01.BringToFront();
            Label labelSs1 = new Label();
            labelSs1.Text = "Sensor X142";
            labelSs1.ForeColor = Color.Black;
            labelSs1.Location = new Point(pnSs_01.Location.X + 10, pnSs_01.Location.Y - 10);
            this.Controls.Add(labelSs1);

            pnSs_03A = new Panel();
            pnSs_03A.BorderStyle = BorderStyle.FixedSingle;
            pnSs_03A.Size = new Size(10, 10);
            pnSs_03A.BackColor = Color.LightGray;
            pnSs_03A.Location = new Point((int)(
                MainConveyor.START_X +
                MainConveyor.L916 +
                MainConveyor.L1800 +
                MainConveyor.L1000 +
                MainConveyor.L560) + 10,
                MainConveyor.START_Y + 2 * MainConveyor.L560 + MainConveyor.L1200 - 5);
            this.Controls.Add(pnSs_03A);
            pnSs_03A.BringToFront();
            Label labelSs3A = new Label();
            labelSs3A.Text = "Sensor X145";
            labelSs3A.ForeColor = Color.Black;
            labelSs3A.Location = new Point(pnSs_03A.Location.X + 10, pnSs_03A.Location.Y + 10);
            this.Controls.Add(labelSs3A);

            pnSs_03B = new Panel();
            pnSs_03B.BorderStyle = BorderStyle.FixedSingle;
            pnSs_03B.Size = new Size(10, 10);
            pnSs_03B.BackColor = Color.LightGray;
            pnSs_03B.Location = new Point((int)(
                MainConveyor.START_X +
                MainConveyor.L916 +
                MainConveyor.L1800 +
                MainConveyor.L1000) + 10,
                MainConveyor.START_Y + 2 * MainConveyor.L560 + MainConveyor.L1200 - 5);
            this.Controls.Add(pnSs_03B);
            pnSs_03B.BringToFront();
            Label labelSs3B = new Label();
            labelSs3B.Text = "Sensor X146";
            labelSs3B.ForeColor = Color.Black;
            labelSs3B.Location = new Point(pnSs_03B.Location.X + 10, pnSs_03B.Location.Y + 10);
            this.Controls.Add(labelSs3B);

            pnSs_05 = new Panel();
            pnSs_05.BorderStyle = BorderStyle.FixedSingle;
            pnSs_05.Size = new Size(10, 10);
            pnSs_05.BackColor = Color.LightGray;
            pnSs_05.Location = new Point((int)(
                MainConveyor.START_X +
                MainConveyor.L916 +
                MainConveyor.L560 +
                MainConveyor.L680) + 10,
                MainConveyor.START_Y - 5);
            this.Controls.Add(pnSs_05);
            pnSs_05.BringToFront();
            Label labelSs5 = new Label();
            labelSs5.Text = "Sensor X202";
            labelSs5.ForeColor = Color.Black;
            labelSs5.Location = new Point(pnSs_05.Location.X + 10, pnSs_05.Location.Y - 10);
            this.Controls.Add(labelSs5);

            pnSs_07A = new Panel();
            pnSs_07A.BorderStyle = BorderStyle.FixedSingle;
            pnSs_07A.Size = new Size(10, 10);
            pnSs_07A.BackColor = Color.LightGray;
            pnSs_07A.Location = new Point((int)(
                MainConveyor.START_X +
                MainConveyor.L916 +
                MainConveyor.L560) + 10,
                MainConveyor.START_Y + 2 * MainConveyor.L560 + MainConveyor.L1200 - 5);

            this.Controls.Add(pnSs_07A);
            pnSs_07A.BringToFront();
            Label labelSs7A = new Label();
            labelSs7A.Text = "Sensor X205";
            labelSs7A.ForeColor = Color.Black;
            labelSs7A.Location = new Point(pnSs_07A.Location.X + 5, pnSs_07A.Location.Y + 10);
            this.Controls.Add(labelSs7A);

            pnSs_07B = new Panel();
            pnSs_07B.BorderStyle = BorderStyle.FixedSingle;
            pnSs_07B.Size = new Size(10, 10);
            pnSs_07B.BackColor = Color.LightGray;
            pnSs_07B.Location = new Point(
                MainConveyor.START_X +
                MainConveyor.L916 + 10,
                MainConveyor.START_Y + 2 * MainConveyor.L560 + MainConveyor.L1200 - 5);
            this.Controls.Add(pnSs_07B);
            pnSs_07B.BringToFront();
            Label labelSs7B = new Label();
            labelSs7B.Text = "Sensor X206";
            labelSs7B.ForeColor = Color.Black;
            labelSs7B.Location = new Point(pnSs_07B.Location.X + 10, pnSs_07B.Location.Y + 10);
            this.Controls.Add(labelSs7B);

            pnSc_01 = new Panel();
            pnSc_01.BorderStyle = BorderStyle.FixedSingle;
            pnSc_01.Size = new Size(15, 15);
            pnSc_01.BackColor = Color.LightGray;
            pnSc_01.Location = new Point((int)(
                MainConveyor.START_X +
                MainConveyor.L916 +
                MainConveyor.L560 +
                MainConveyor.L680 +
                MainConveyor.L560 +
                MainConveyor.L1000 +
                MainConveyor.L560 +
                MainConveyor.L680 +
                MainConveyor.L560) + 40,
                (int)(MainConveyor.START_Y + MainConveyor.L560) + 20);
            this.Controls.Add(pnSc_01);
            Label labelScanner1 = new Label();
            labelScanner1.Text = "Scanner 01";
            labelScanner1.ForeColor = Color.Black;
            labelScanner1.Location = new Point(pnSc_01.Location.X + 20, pnSc_01.Location.Y);
            this.Controls.Add(labelScanner1);

            pnSs_01sc = new Panel();
            pnSs_01sc.BorderStyle = BorderStyle.FixedSingle;
            pnSs_01sc.Size = new Size(10, 10);
            pnSs_01sc.BackColor = Color.LightGray;
            pnSs_01sc.Location = new Point((int)(
                MainConveyor.START_X +
                MainConveyor.L916 +
                MainConveyor.L1800 +
                MainConveyor.L1000 +
                MainConveyor.L1800) + 25,
                MainConveyor.START_Y + MainConveyor.L560 - 5);
            this.Controls.Add(pnSs_01sc);
            pnSs_01sc.BringToFront();

            pnSc_02 = new Panel();
            pnSc_02.BorderStyle = BorderStyle.FixedSingle;
            pnSc_02.Size = new Size(15, 15);
            pnSc_02.BackColor = Color.LightGray;
            pnSc_02.Location = new Point((int)(
                MainConveyor.START_X +
                MainConveyor.L916 +
                MainConveyor.L560 +
                MainConveyor.L680 +
                MainConveyor.L560) +
                40, (int)(MainConveyor.START_Y + MainConveyor.L560) + 20);
            this.Controls.Add(pnSc_02);
            Label labelScanner2 = new Label();
            labelScanner2.Text = "Scanner 02";
            labelScanner2.ForeColor = Color.Black;
            labelScanner2.Location = new Point(pnSc_02.Location.X + 20, pnSc_02.Location.Y);
            this.Controls.Add(labelScanner2);

            pnSs_02sc = new Panel();
            pnSs_02sc.BorderStyle = BorderStyle.FixedSingle;
            pnSs_02sc.Size = new Size(10, 10);
            pnSs_02sc.BackColor = Color.LightGray;
            pnSs_02sc.Location = new Point((int)(
                MainConveyor.START_X +
                MainConveyor.L916 +
                MainConveyor.L1800) + 25,
                MainConveyor.START_Y + MainConveyor.L560 - 5);
            this.Controls.Add(pnSs_02sc);
            pnSs_02sc.BringToFront();

            DisplayDataGridView(dgvLogSent, Color.WhiteSmoke);
            DisplayDataGridView(dgvLogReceived, Color.WhiteSmoke);
        }

        private void RegisterEvents()
        {
            Load += HomeForm_Load;
            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
            btnReset.Click += BtnReset_Click;
            btnRoutingMode.Click += BtnRoutingMode_Click;
            btnSortingMode.Click += BtnSortingMode_Click;
            btnGTP_01_Status.Click += BtnGTP_01_Status_Click;
            btnGTP_02_Status.Click += BtnGTP_02_Status_Click;

            //Logs
            Client.Client.OnLogSentMessage += Client_OnLogSentMessage;
            Grapher.OnLogReceivedMessage += Grapher_OnLogReceivedMessage;

            Client.Client.CodeResult += Client_CodeResult;
            Grapher.CodeResult += Grapher_CodeResult;
            Grapher.ReconnectTo += Grapher_ReconnectTo;
            Grapher.OnShowMode += Grapher_OnShowMode;
            WcsForm.OnConnection += WcsForm_OnConnection;

            Client.Client.OnDisconnect += Client_OnDisconnect;
            PLC.OnClosed += PLC_OnClosed;
        }


        private void InitializeTimer()
        {
            tmrDisplayContainer = new System.Windows.Forms.Timer();
            tmrDisplayContainer.Tick += TmrDisplayContainer_Tick;
            tmrDisplayContainer.Interval = 250;
            tmrDisplayContainer.Start();

            tmrDisplayDataControl = new System.Windows.Forms.Timer();
            tmrDisplayDataControl.Tick += TmrDisplayDataControl_Tick;
            tmrDisplayDataControl.Interval = 500;
            tmrDisplayDataControl.Start();

            tmrDisplayNote = new System.Windows.Forms.Timer();
            tmrDisplayNote.Tick += TmrDisplayNote_Tick;
            tmrDisplayNote.Interval = 800;
            tmrDisplayNote.Start();

            tmrRemoveContainer = new System.Windows.Forms.Timer();
            tmrRemoveContainer.Tick += TmrRemoveContainer_Tick;
            tmrRemoveContainer.Interval = 8000;
            tmrRemoveContainer.Start();
        }

        private void InitializeConnection()
        {
            WesSocket = new Client.Client();
            Scanner01Socket = new Client.Client();
            Scanner02Socket = new Client.Client();
            Plc = new PLC();
            grapher = new Grapher();
        }

        #endregion


        #region Threads
        private Thread loadingThread = null;
        #endregion


        #region Events
        private void HomeForm_Load(object? sender, EventArgs e)
        {
            loadingThread = new Thread(() =>
            {
                using (LoadingForm connForm = new LoadingForm(ConnectToScanner01, "Connecting to Scanner 01"))
                {
                    Invoke((MethodInvoker)delegate
                    {
                        connForm.ShowDialog(this);
                    });
                }

                using (LoadingForm connForm = new LoadingForm(ConnectToScanner02, "Connecting to Scanner 02"))
                {
                    Invoke((MethodInvoker)delegate
                    {
                        connForm.ShowDialog(this);
                    });
                }

                using (LoadingForm connForm = new LoadingForm(ConnectToPLC, "Connecting to PLC"))
                {
                    Invoke((MethodInvoker)delegate
                    {
                        connForm.ShowDialog(this);
                    });
                }

                using (LoadingForm connForm = new LoadingForm(() => ConnectToWES(0), "Connecting to WES"))
                {
                    Invoke((MethodInvoker)delegate
                    {
                        connForm.ShowDialog(this);
                    });
                }
            });
            loadingThread.IsBackground = true;
            loadingThread.Start();

        }

        private void WcsForm_OnConnection(int options)
        {
            switch (options)
            {
                case 0:
                    using (LoadingForm connForm = new LoadingForm(() => ConnectToWES(0), "Connecting to WES"))
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            connForm.ShowDialog(this);
                        });
                    }
                    break;
                case 1:
                    DisconnectToWES();
                    break;
                case 2:
                    using (LoadingForm connForm = new LoadingForm(ConnectToScanner01, "Connecting to Scanner 01"))
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            connForm.ShowDialog(this);
                        });
                    }
                    break;
                case 3:
                    DisconnectToScanner01();
                    break;
                case 4:
                    using (LoadingForm connForm = new LoadingForm(ConnectToScanner02, "Connecting to Scanner 02"))
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            connForm.ShowDialog(this);
                        });
                    }
                    break;
                case 5:
                    DisconnectToScanner02();
                    break;
                case 6:
                    using (LoadingForm connForm = new LoadingForm(ConnectToPLC, "Connecting to PLC"))
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            connForm.ShowDialog(this);
                        });
                    }
                    break;
                case 7:
                    DisconnectToPLC();
                    break;
                default:
                    break;
            }
        }

        private void PLC_OnClosed(int iReturnCode)
        {
            try
            {
                Invoke(new Action(() =>
                    {
                        DialogResult dr = MessageBox.Show("Loss connection to PLC. Do you want to retry?", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        if (dr == DialogResult.Retry)
                        {
                            ConnectToPLC();
                            ConnectToScanner01();
                            ConnectToScanner02();
                        }
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Client_OnDisconnect(Client.Client client)
        {
            try
            {
                lock (client)
                {
                    if (client.Port == scanner01Config.Port)
                    {
                        DisplayConnectionStatus(pnScanner01, lbScanner01, StatusEnum.Error);
                        OnShowReady("Scanner 01 is offline.", Color.Orange);
                    }
                    else if (client.Port == scanner02Config.Port)
                    {
                        DisplayConnectionStatus(pnScanner02, lbScanner02, StatusEnum.Error);
                        OnShowReady("Scanner 02 is offline.", Color.Orange);
                    }
                    else if (client.Port == wesConfig.Port)
                    {
                        DisplayConnectionStatus(pnWES, lbWES, StatusEnum.Error);
                        OnShowReady("WES is offline.", Color.Orange);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnGTP_02_Status_Click(object? sender, EventArgs e)
        {
            grapher.GTP_02_STATUS = StatusEnum.Ready;
        }

        private void BtnGTP_01_Status_Click(object? sender, EventArgs e)
        {
            grapher.GTP_01_STATUS = StatusEnum.Ready;
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            ClearAllContainer();
            Plc.WriteSystem(PlcEnum.RESET);
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            Plc.WriteSystem(PlcEnum.STOP);
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            Plc.WriteSystem(PlcEnum.START);
        }

        private void BtnSortingMode_Click(object? sender, EventArgs e)
        {
            ClearAllContainer();
            Plc.WriteMode(PlcEnum.SORTING_MODE);
        }

        private void BtnRoutingMode_Click(object? sender, EventArgs e)
        {
            ClearAllContainer();
            Plc.WriteMode(PlcEnum.ROUTING_MODE);
        }

        private void Grapher_OnShowMode(PlcEnum mode)
        {
            Invoke(new Action(() =>
            {
                if (mode == PlcEnum.ROUTING_MODE)
                {
                    btnRoutingMode.BackColor = Color.LimeGreen;
                    btnRoutingMode.ForeColor = Color.White;

                    btnSortingMode.BackColor = Color.Transparent;
                    btnSortingMode.ForeColor = Color.Gray;
                }
                else if (mode == PlcEnum.SORTING_MODE)
                {
                    btnRoutingMode.BackColor = Color.Transparent;
                    btnRoutingMode.ForeColor = Color.Gray;

                    btnSortingMode.BackColor = Color.Orange;
                    btnSortingMode.ForeColor = Color.White;
                }
            }));
        }

        private void Client_CodeResult(string result, int scannerNumber)
        {
            Invoke(new Action(() =>
            {
                if (scannerNumber == 1)
                { //show result read code of scanner 01
                    txtS1.Text = result;
                    if (result != AppConfig.NG)
                    {
                        txtS1.ForeColor = Color.Green;
                        pnSc_01.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        txtS1.ForeColor = Color.Red;
                        pnSc_01.BackColor = Color.Red;
                        note_ng = true;
                    }

                    //Injecting
                    Models.Container container = new Models.Container();
                    container.Barcode = result;
                    if (grapher.Containers[0, 1].Count < grapher.MaxContainer[0, 1])
                    {
                        grapher.Containers[0, 1].Add(container);
                        grapher.CallRequest(new ContainerRouteRequest(), 0, 1, AppConfig.SCANNER_01);
                    }
                    else
                        txtS1.ForeColor = Color.Orange;
                }
                else if (scannerNumber == 2)
                { //show result read code scanner 02
                    txtS2.Text = result;
                    if (result != AppConfig.NG)
                    {
                        txtS2.ForeColor = Color.Green;
                        pnSc_02.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        txtS2.ForeColor = Color.Red;
                        pnSc_02.BackColor= Color.Red;
                        note_ng = true;
                    }
                    grapher.CallRequest(new ContainerRouteRequest(), 2, 7, AppConfig.SCANNER_02);
                }
            }));
        }

        /// <summary>
        /// Show result read code of the grapher
        /// </summary>
        /// <param name="result"></param>
        private void Grapher_CodeResult(string result)
        {
            Invoke(new Action(() =>
            {
                txtS2.Text = result;
                if (result != AppConfig.NG)
                {
                    txtS2.ForeColor = Color.Green;
                    pnSc_02.BackColor = Color.LightGreen;
                }
                else
                {
                    txtS2.ForeColor = Color.Red;
                    pnSc_02.BackColor = Color.Red;
                    note_ng = true;
                }
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">0: Retry, 1: Cancel</param>
        private void Grapher_ReconnectTo(int options)
        {
            WesSocket.Disconnect();
            Invoke((MethodInvoker)delegate
            {
                if (options == 0)
                {
                    loadingThread = new Thread(() =>
                    {
                        using (LoadingForm connForm = new LoadingForm(() => ConnectToWES(0), "Connecting to WCS"))
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                connForm.ShowDialog(this);
                            });
                        }
                    });
                    loadingThread.IsBackground = true;
                    loadingThread.Start();
                }
                else
                    DisplayConnectionStatus(pnWES, lbWES, StatusEnum.Disconnected);
            });
        }

        /// <summary>
        /// Write log on GUI, Log sent may be RequestMessage or ResponseMessage
        /// </summary>
        /// <param name="msg">Message to write</param>
        private void Client_OnLogSentMessage(MessageBase msg)
        {
            try
            {
                DataGridViewRow row = (DataGridViewRow)dgvLogSent.Rows[0].Clone();
                Type type = msg.GetType();
                if (type == typeof(RequestMessage))
                {
                    RequestMessage requestMessage = (RequestMessage)msg;
                    row.Cells[0].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    row.Cells[1].Value = requestMessage.messageType;
                    row.Cells[2].Value = requestMessage.sequenceNumber;

                    if (requestMessage.data != null)
                    {
                        Type typeData = requestMessage.data.GetType();
                        if (typeData == typeof(ContainerRouteRequest))
                        {
                            ContainerRouteRequest data = (ContainerRouteRequest)requestMessage.data;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[9].Value = data.scannerName;
                        }
                        else if (typeData == typeof(DeviceCommandRequest))
                        {
                            DeviceCommandRequest data = (DeviceCommandRequest)requestMessage.data;
                            row.Cells[5].Value = data.command;
                            row.Cells[8].Value = data.deviceId;
                        }
                        else if (typeData == typeof(ContainerScanRequest))
                        {
                            ContainerScanRequest data = (ContainerScanRequest)requestMessage.data;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[8].Value = data.deviceId;
                        }
                        else if (typeData == typeof(ContainerDivert))
                        {
                            ContainerDivert data = (ContainerDivert)requestMessage.data;
                            row.Cells[9].Value = data.scannerName;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[8].Value = data.deviceId;
                        }
                        else if (typeData == typeof(CurrentSequenceRequest))
                        {
                            CurrentSequenceRequest data = (CurrentSequenceRequest)requestMessage.data;

                        }
                    }
                }
                else if (type == typeof(ResponseMessage))
                {
                    ResponseMessage responseMessage = (ResponseMessage)msg;
                    row.Cells[0].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    row.Cells[1].Value = responseMessage.messageType;
                    row.Cells[2].Value = responseMessage.sequenceNumber;
                    row.Cells[3].Value = responseMessage.codeReturn;
                    row.Cells[4].Value = responseMessage.message;

                    if (responseMessage.data != null)
                    {
                        Type typeData = responseMessage.data.GetType();
                        if (typeData == typeof(ContainerRouteResponse))
                        {
                            ContainerRouteResponse data = (ContainerRouteResponse)responseMessage.data;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[8].Value = data.deviceId;
                            row.Cells[9].Value = data.scannerName;
                        }
                        else if (typeData == typeof(DeviceStatusResponse))
                        {
                            DeviceStatusResponse data = (DeviceStatusResponse)responseMessage.data;
                            row.Cells[6].Value = data.status;
                            row.Cells[8].Value = data.deviceId;
                        }
                        else if (typeData == typeof(ContainerScanResponse))
                        {
                            ContainerScanResponse data = (ContainerScanResponse)responseMessage.data;
                            row.Cells[8].Value = data.deviceId;
                        }
                        else if (typeData == typeof(ContainerDivert))
                        {
                            ContainerDivert data = (ContainerDivert)responseMessage.data;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[8].Value = data.deviceId;
                            row.Cells[9].Value = data.scannerName;
                        }
                    }
                }

                dgvLogSent.Invoke(new Action(() =>
                {
                    dgvLogSent.Rows.Insert(0, row);
                }));
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Write log on GUI, Log received may be RequestMessage or ResponseMessage
        /// </summary>
        /// <param name="msg"></param>
        private void Grapher_OnLogReceivedMessage(MessageBase msg)
        {
            try
            {
                DataGridViewRow row = (DataGridViewRow)dgvLogReceived.Rows[0].Clone();
                Type type = msg.GetType();
                if (type == typeof(RequestMessage))
                {
                    RequestMessage requestMessage = (RequestMessage)msg;
                    row.Cells[0].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    row.Cells[1].Value = requestMessage.messageType;

                    if (requestMessage.data != null)
                    {
                        Type typeData = requestMessage.data.GetType();
                        if (typeData == typeof(ContainerRouteRequest))
                        {
                            ContainerRouteRequest data = (ContainerRouteRequest)requestMessage.data;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[9].Value = data.scannerName;
                        }
                        else if (typeData == typeof(ContainerScanRequest))
                        {
                            ContainerScanRequest data = (ContainerScanRequest)requestMessage.data;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[8].Value = data.deviceId;
                        }
                        else if (typeData == typeof(ContainerDivert))
                        {
                            ContainerDivert data = (ContainerDivert)requestMessage.data;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[8].Value = data.deviceId;
                            row.Cells[9].Value = data.scannerName;
                        }
                    }
                }
                else if (type == typeof(ResponseMessage))
                {
                    ResponseMessage responseMessage = (ResponseMessage)msg;
                    row.Cells[0].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    row.Cells[1].Value = responseMessage.messageType;
                    row.Cells[2].Value = responseMessage.sequenceNumber;
                    row.Cells[3].Value = responseMessage.codeReturn;
                    row.Cells[4].Value = responseMessage.message;

                    if (responseMessage.data != null)
                    {
                        Type typeData = responseMessage.data.GetType();
                        if (typeData == typeof(ContainerRouteResponse))
                        {
                            ContainerRouteResponse data = (ContainerRouteResponse)responseMessage.data;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[8].Value = data.deviceId;
                            row.Cells[9].Value = data.scannerName;
                        }
                        else if (typeData == typeof(DeviceStatusResponse))
                        {
                            DeviceStatusResponse data = (DeviceStatusResponse)responseMessage.data;
                            row.Cells[6].Value = data.status;
                            row.Cells[8].Value = data.deviceId;
                        }
                        else if (typeData == typeof(DeviceCommandRequest))
                        {
                            DeviceCommandRequest data = (DeviceCommandRequest)responseMessage.data;
                            row.Cells[5].Value = data.command;
                            row.Cells[8].Value = data.deviceId;
                        }
                        else if (typeData == typeof(ContainerScanResponse))
                        {
                            ContainerScanResponse data = (ContainerScanResponse)responseMessage.data;
                            row.Cells[8].Value = data.deviceId;
                        }
                        else if (typeData == typeof(ContainerDivert))
                        {
                            ContainerDivert data = (ContainerDivert)responseMessage.data;
                            row.Cells[7].Value = data.barcode;
                            row.Cells[8].Value = data.deviceId;
                            row.Cells[9].Value = data.scannerName;
                        }
                        else if (typeData == typeof(CurrentSequenceResponse))
                        {
                            CurrentSequenceResponse data = (CurrentSequenceResponse)responseMessage.data;
                            row.Cells[10].Value = data.currentSequenceNumber;
                        }
                    }
                }

                dgvLogReceived.Invoke(new Action(() =>
                {
                    dgvLogReceived.Rows.Insert(0, row);
                }));
            }
            catch (Exception ex)
            {

            }
        }

        #endregion


        #region Methods

        public void ClearAllContainer()
        {
            for (int i = 0; i < AppConfig.SIZE_OF_CONTAINER; i++)
                for (int j = 0; j < AppConfig.SIZE_OF_CONTAINER; j++)
                    if (grapher.Containers[i, j].Count > 0)
                        grapher.Containers[i, j].Clear();

        }

        public void ConnectToWES(int k)
        {
            if (WesSocket == null)
            {
                MessageBox.Show("Wes socket has not been initialized");
                return;
            }
            if (WesSocket.Status == StatusEnum.Connected) return;

            if (k >= wesConfig.AutoReconnectTimes)
            {
                OnShowReady("Lost WES connection.", Color.Red);
                DialogResult dr = MessageBox.Show("Lost WES connection. Do you want to retry?", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (dr == DialogResult.Retry)
                {
                    ConnectToWES(0);
                }
                else if (dr == DialogResult.Cancel)
                {
                    DisplayConnectionStatus(pnWES, lbWES, StatusEnum.Disconnected);
                    OnShowReady("WES is offline.", Color.Orange);
                }
            }
            else
            {
                try
                {
                    WesSocket.Connect(wesConfig.IpAddress, wesConfig.Port, 0);
                    DisplayConnectionStatus(pnWES, lbWES, WesSocket.Status);
                    CheckSystemAlready();

                    grapher.StartResponseMessageProcessing();
                    grapher.CallCurrentSequenceRequest();
                    return;
                }
                catch (Exception ex)
                {
                    DisplayConnectionStatus(pnWES, lbWES, StatusEnum.Error);
                    switch (k)
                    {
                        case 0:
                            OnShowReady("Automatically reconnect once.", Color.DarkSlateBlue);
                            break;
                        case 1:
                            OnShowReady("Automatically reconnect twice.", Color.DarkSlateBlue);
                            break;
                        default:
                            OnShowReady("Automatically reconnect " + (k + 1).ToString() + " times.", Color.DarkSlateBlue);
                            break;
                    }

                    Thread.Sleep(wesConfig.AutoReconnectInterval);
                    ConnectToWES(k + 1);
                }
            }
        }
        public void DisconnectToWES()
        {
            if (WesSocket == null)
            {
                MessageBox.Show("WES socket has not been initialized");
                return;
            }
            if (WesSocket.Status == StatusEnum.Disconnected) return;

            try
            {
                WesSocket.Disconnect();
                DisplayConnectionStatus(pnWES, lbWES, WesSocket.Status);
                OnShowReady("WES is offline.", Color.Orange);
            }
            catch (Exception ex)
            {
                DisplayConnectionStatus(pnWES, lbWES, StatusEnum.Error);
                MessageBox.Show(ex.Message);
            }
        }

        private void ConnectToScanner01()
        {
            if (Scanner01Socket == null)
            {
                MessageBox.Show("Scanner 01 Socket has not been initialized.");
                return;
            }

            if (Scanner01Socket.Status == StatusEnum.Connected) return;

            try
            {
                Scanner01Socket.Connect(scanner01Config.IpAddress, scanner01Config.Port, 1);
                DisplayConnectionStatus(pnScanner01, lbScanner01, Scanner01Socket.Status);
                CheckSystemAlready();
            }
            catch (Exception ex)
            {
                DisplayConnectionStatus(pnScanner01, lbScanner01, StatusEnum.Error);
                DialogResult dr = MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (dr == DialogResult.Retry)
                {
                    ConnectToScanner01();
                }
                else if (dr == DialogResult.Cancel)
                {
                    DisconnectToScanner01();
                }
            }
        }
        private void DisconnectToScanner01()
        {
            if (Scanner01Socket == null)
            {
                MessageBox.Show("Scanner 01 Socket has not been initialized.");
                return;
            }
            if (Scanner01Socket.Status == StatusEnum.Disconnected)
            {
                OnShowReady("Scanner 01 is offline.", Color.Orange);
                DisplayConnectionStatus(pnScanner01, lbScanner01, Scanner01Socket.Status);
                return;
            }

            try
            {
                Scanner01Socket.Disconnect();
                DisplayConnectionStatus(pnScanner01, lbScanner01, Scanner01Socket.Status);
                OnShowReady("Scanner 01 is offline.", Color.Orange);
            }
            catch (Exception ex)
            {
                DisplayConnectionStatus(pnScanner01, lbScanner01, StatusEnum.Error);
                MessageBox.Show(ex.Message);
            }
        }

        private void ConnectToScanner02()
        {
            if (Scanner02Socket == null)
            {
                MessageBox.Show("Scanner 02 Socket has not been initialized.");
                return;
            }

            if (Scanner02Socket.Status == StatusEnum.Connected) return;

            try
            {
                Scanner02Socket.Connect(scanner02Config.IpAddress, scanner02Config.Port, 2);
                DisplayConnectionStatus(pnScanner02, lbScanner02, StatusEnum.Connected);
                CheckSystemAlready();
            }
            catch (Exception ex)
            {
                DisplayConnectionStatus(pnScanner02, lbScanner02, StatusEnum.Error);
                DialogResult dr = MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (dr == DialogResult.Retry)
                {
                    ConnectToScanner02();
                }
                else if (dr == DialogResult.Cancel)
                {
                    DisconnectToScanner02();
                }
            }
        }
        private void DisconnectToScanner02()
        {
            if (Scanner02Socket == null)
            {
                MessageBox.Show("Scanner 02 Socket has not been initialized.");
                return;
            }
            if (Scanner02Socket.Status == StatusEnum.Disconnected)
            {
                OnShowReady("Scanner 02 is offline.", Color.Orange);
                DisplayConnectionStatus(pnScanner02, lbScanner02, Scanner02Socket.Status);
                return;
            }

            try
            {
                Scanner02Socket.Disconnect();
                DisplayConnectionStatus(pnScanner02, lbScanner02, Scanner02Socket.Status);
                OnShowReady("Scanner 02 is offline.", Color.Orange);
            }
            catch (Exception ex)
            {
                DisplayConnectionStatus(pnScanner02, lbScanner02, StatusEnum.Error);
                MessageBox.Show(ex.Message);
            }
        }

        private void ConnectToPLC()
        {
            if (Plc == null)
            {
                MessageBox.Show("Plc has not been initialized.");
                return;
            }
            if (Plc.Status == StatusEnum.Connected) return;

            try
            {
                Plc.Open();
                if (Plc.Status == StatusEnum.Connected)
                {
                    DisplayConnectionStatus(pnPLC, lbPLC, StatusEnum.Connected);
                    grapher.StartDataControlProcessing();
                    CheckSystemAlready();
                }
                else if (Plc.Status == StatusEnum.Error)
                {
                    DisplayConnectionStatus(pnPLC, lbPLC, StatusEnum.Error);
                    DialogResult dr = MessageBox.Show("Connection failed to PLC. Code error: " + String.Format("0x{0:x8}", Plc.IReturnCode).ToUpper(), "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (dr == DialogResult.Retry)
                    {
                        ConnectToPLC();
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        DisconnectToPLC();
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayConnectionStatus(pnPLC, lbPLC, StatusEnum.Error);
                MessageBox.Show(ex.Message);
            }
        }
        private void DisconnectToPLC()
        {
            if (Plc == null)
            {
                MessageBox.Show("Plc has not been initialized");
                return;
            }
            if (Plc.Status == StatusEnum.Disconnected) return;

            try
            {
                Plc.Close();
                if (Plc.Status == StatusEnum.Disconnected)
                {
                    DisplayConnectionStatus(pnPLC, lbPLC, Plc.Status);
                    OnShowReady("PLC is offline.", Color.Orange);
                    grapher.Stop();
                }
            }
            catch (Exception ex)
            {
                DisplayConnectionStatus(pnPLC, lbPLC, StatusEnum.Error);
                MessageBox.Show(ex.Message);
            }
        }

        public void DisplayConnectionStatus(CircularPanel pn, Label lb, StatusEnum status)
        {
            Invoke((MethodInvoker)delegate
            {
                if (status == StatusEnum.Connected)
                {
                    pn.BackColor = Color.LimeGreen;
                    lb.ForeColor = Color.Green;
                    lb.Text = status.ToString();
                    lb.Font = new Font("Siemens Sans", 12, FontStyle.Regular);
                }
                else if (status == StatusEnum.Disconnected)
                {
                    pn.BackColor = Color.WhiteSmoke;
                    lb.ForeColor = Color.Black;
                    lb.Text = status.ToString();
                    lb.Font = new Font("Siemens Sans", 12, FontStyle.Regular);
                }
                else if (status == StatusEnum.Error)
                {
                    pn.BackColor = Color.Red;
                    lb.ForeColor = Color.Red;
                    lb.Text = status.ToString();
                    lb.Font = new Font("Siemens Sans", 12, FontStyle.Regular);
                }
            });


        }

        private void DisplayDataGridView(DataGridView dgv, Color color)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = color;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
        }
        #endregion


        #region Timers
        private void TmrDisplayContainer_Tick(object? sender, EventArgs e) => DisplayContainer();

        private void TmrDisplayDataControl_Tick(object? sender, EventArgs e) => DisplayDataControl();

        private void TmrRemoveContainer_Tick(object? sender, EventArgs e) => RemoveContainer();



        int[,] memCount = new int[AppConfig.SIZE_OF_CONTAINER, AppConfig.SIZE_OF_CONTAINER];
        private void DisplayContainer()
        {
            if (grapher == null) return;
            for (int i = 0; i < AppConfig.SIZE_OF_CONTAINER; i++)
                for (int j = 0; j < AppConfig.SIZE_OF_CONTAINER; j++)
                    if (grapher.Containers[i, j].Count > 0)
                    {
                        if (grapher.Containers[i, j].Count != memCount[i, j])
                        {
                            memCount[i, j] = grapher.Containers[i, j].Count;
                            for (int k = 0; k < grapher.Containers[i, j].Count; k++)
                            {
                                if (k > containerPanels[i, j].Count - 1)
                                {
                                    Debug.WriteLine(grapher.Containers[i, j].Count - containerPanels[i, j].Count);
                                }
                                else
                                {
                                    containerPanels[i, j][k].container = grapher.Containers[i, j][k];
                                    containerPanels[i, j][k].indexOfContainer = new int[3] { i, j, k };
                                    containerPanels[i, j][k].Visible = true;
                                    if (k >= grapher.Containers[i, j].Count - 1)
                                        for (int m = k + 1; m <= containerPanels[i, j].Count - 1; m++)
                                            containerPanels[i, j][m].Visible = false;
                                }
                            }
                        }
                        else memCount[i, j] = 0;
                    }
                    else
                        for (int k = 0; k < containerPanels[i, j].Count; k++)
                            containerPanels[i, j][k].Visible = false;

            //Show the container in OP location when the sensor ON
            if (grapher.DataControlReceived.Mode == PlcEnum.SORTING_MODE)
            {
                if (grapher.Containers[4, 5].Count > 0 && grapher.DataControlReceived.Sensors[2])//OP GTP 01 
                {
                    containerPanels[4, 5][0].Visible = true;
                }
                else containerPanels[4, 5][0].Visible = false;

                if (grapher.Containers[10, 11].Count > 0 && grapher.DataControlReceived.Sensors[5])//OP GTP 02
                {
                    containerPanels[10, 11][0].Visible = true;
                }
                else containerPanels[10, 11][0].Visible = false;
            }
        }

        private void DisplayDataControl()
        {
            if (Plc != null)
            {
                if (Plc.Status == StatusEnum.Connected)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (Plc.DataControl.Stoppers[i])
                        {
                            DisplayDeviceStatus(stopperPanels[i], true, 0, i);
                        }
                        else
                        {
                            DisplayDeviceStatus(stopperPanels[i], false, 0, i);
                        }

                        if (Plc.DataControl.Pushers[i])
                        {
                            DisplayDeviceStatus(pusherPanels[i], true, 1, i);
                        }
                        else
                        {
                            DisplayDeviceStatus(pusherPanels[i], false, 1, i);
                        }

                        if (Plc.DataControl.Sensors[i])
                        {
                            DisplayDeviceStatus(sensorPanels[i], true, 2, i);
                        }
                        else
                        {
                            DisplayDeviceStatus(sensorPanels[i], false, 2, i);
                        }
                    }

                    if (Plc.DataControl.Running == 1)
                    {
                        btnStart.BackColor = Color.LimeGreen;
                        btnStart.ForeColor = Color.White;
                    }
                    else
                    {
                        btnStart.BackColor = Color.Transparent;
                        btnStart.ForeColor = SystemColors.WindowFrame;
                    }

                    if (Plc.DataControl.Pause == 1)
                    {
                        btnStop.BackColor = Color.Orange;
                        btnStop.ForeColor = Color.White;
                    }
                    else
                    {
                        btnStop.BackColor = Color.Transparent;
                        btnStop.ForeColor = SystemColors.WindowFrame;
                    }

                    if (Plc.DataControl.Reset == 1)
                    {
                        btnReset.BackColor = Color.Yellow;
                        btnReset.ForeColor = Color.Red;
                    }
                    else
                    {
                        btnReset.BackColor = Color.Transparent;
                        btnReset.ForeColor = SystemColors.WindowFrame;
                    }

                    if(Plc.DataControl.Emg01 == 1)
                    {
                        btnEMG01.BackColor = Color.Orange;
                    }
                    else
                    {
                        btnEMG01.BackColor = Color.Silver;
                    }

                    if (Plc.DataControl.Emg02 == 1)
                    {
                        btnEMG02.BackColor = Color.Orange;
                    }
                    else
                    {
                        btnEMG02.BackColor = Color.Silver;
                    }
                }

                DisplayConnectionStatus(pnPLC, lbPLC, Plc.Status);
            }
                            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pn"></param>
        /// <param name="status"></param>
        /// <param name="options">deviece type(Stopper, Pusher, Sensor)</param>
        /// <param name="i">device at index</param>
        private void DisplayDeviceStatus(Panel pn, bool status, int options, int i)
        {
            Color stopperColor = Color.Yellow;
            Color pusherColor = Color.LimeGreen;
            Color sensorColor = Color.Orange;
            Color turnOffColor = Color.LightGray;

            if (status) //Device ON
            {
                pn.BackColor = Color.Orange;//show in status systems

                switch (i)//show on Conveyor
                {
                    case 0:
                        if (options == 0)
                        {
                            mainConveyor.pnStopperRight.BackColor = stopperColor;
                        }
                        else if (options == 1)
                        {
                            mainConveyor.pnPusher01a.BackColor = pusherColor;
                            mainConveyor.pnPusher01b.BackColor = pusherColor;
                            mainConveyor.pnPusher01c.BackColor = pusherColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_01.BackColor = sensorColor;
                        }
                        break;
                    case 1:
                        if (options == 0)
                        {
                            verticalConveyor_GTP1_Right.pnStopperBottom.BackColor = stopperColor;
                        }
                        else if (options == 1)
                        {
                            horizoltalConveyor_GTP1.pnPusherRightA.BackColor = pusherColor;
                            horizoltalConveyor_GTP1.pnPusherRightB.BackColor = pusherColor;
                            horizoltalConveyor_GTP1.pnPusherRightC.BackColor = pusherColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_03A.BackColor = sensorColor;
                        }
                        break;
                    case 2:
                        if (options == 0) // stoppers
                        {
                            horizoltalConveyor_GTP1.pnStopper.BackColor = stopperColor;
                        }
                        else if (options == 1) // pushers
                        {
                            horizoltalConveyor_GTP1.pnPusherLeftA.BackColor = pusherColor;
                            horizoltalConveyor_GTP1.pnPusherLeftB.BackColor = pusherColor;
                            horizoltalConveyor_GTP1.pnPusherLeftC.BackColor = pusherColor;
                        }
                        else if (options == 2) //sensors
                        {
                            pnSs_03B.BackColor = sensorColor;
                        }
                        break;
                    case 3:
                        if (options == 0)
                        {
                            verticalConveyor_GTP1_Left.pnStopperTop.BackColor = stopperColor;
                        }
                        else if (options == 1)
                        {
                            mainConveyor.pnPusher04a.BackColor = pusherColor;
                            mainConveyor.pnPusher04b.BackColor = pusherColor;
                            mainConveyor.pnPusher04c.BackColor = pusherColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_05.BackColor = sensorColor;
                        }
                        break;
                    case 4:
                        if (options == 0)
                        {
                            mainConveyor.pnStopperLeft.BackColor = stopperColor;
                        }
                        else if (options == 1)
                        {
                            mainConveyor.pnPusher05a.BackColor = pusherColor;
                            mainConveyor.pnPusher05b.BackColor = pusherColor;
                            mainConveyor.pnPusher05c.BackColor = pusherColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_07A.BackColor = sensorColor;
                        }
                        break;
                    case 5:
                        if (options == 0)
                        {
                            verticalConveyor_GTP2_Right.pnStopperBottom.BackColor = stopperColor;
                        }
                        else if (options == 1)
                        {
                            horizoltalConveyor_GTP2.pnPusherRightA.BackColor = pusherColor;
                            horizoltalConveyor_GTP2.pnPusherRightB.BackColor = pusherColor;
                            horizoltalConveyor_GTP2.pnPusherRightC.BackColor = pusherColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_07B.BackColor = sensorColor;
                        }
                        break;
                    case 6:
                        if (options == 0)
                        {
                            horizoltalConveyor_GTP2.pnStopper.BackColor = stopperColor;
                        }
                        else if (options == 1)
                        {
                            horizoltalConveyor_GTP2.pnPusherLeftA.BackColor = pusherColor;
                            horizoltalConveyor_GTP2.pnPusherLeftB.BackColor = pusherColor;
                            horizoltalConveyor_GTP2.pnPusherLeftC.BackColor = pusherColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_01sc.BackColor = sensorColor;
                        }
                        break;
                    case 7:
                        if (options == 0)
                        {
                            verticalConveyor_GTP2_Left.pnStopperTop.BackColor = stopperColor;
                        }
                        else if (options == 1)
                        {
                            mainConveyor.pnPusher08a.BackColor = pusherColor;
                            mainConveyor.pnPusher08b.BackColor = pusherColor;
                            mainConveyor.pnPusher08c.BackColor = pusherColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_02sc.BackColor = sensorColor;
                        }
                        break;
                    default:
                        break;
                }
            }
            else // Device OFF
            {
                pn.BackColor = SystemColors.ControlLight;
                switch (i)
                {
                    case 0:
                        if (options == 0)
                        {
                            mainConveyor.pnStopperRight.BackColor = turnOffColor;
                        }
                        else if (options == 1)
                        {
                            mainConveyor.pnPusher01a.BackColor = turnOffColor;
                            mainConveyor.pnPusher01b.BackColor = turnOffColor;
                            mainConveyor.pnPusher01c.BackColor = turnOffColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_01.BackColor = turnOffColor;
                        }
                        break;
                    case 1:
                        if (options == 0)
                        {
                            verticalConveyor_GTP1_Right.pnStopperBottom.BackColor = turnOffColor;
                        }
                        else if (options == 1)
                        {
                            horizoltalConveyor_GTP1.pnPusherRightA.BackColor = turnOffColor;
                            horizoltalConveyor_GTP1.pnPusherRightB.BackColor = turnOffColor;
                            horizoltalConveyor_GTP1.pnPusherRightC.BackColor = turnOffColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_03A.BackColor = turnOffColor;
                        }
                        break;
                    case 2:
                        if (options == 0)
                        {
                            horizoltalConveyor_GTP1.pnStopper.BackColor = turnOffColor;
                        }
                        else if (options == 1)
                        {
                            horizoltalConveyor_GTP1.pnPusherLeftA.BackColor = turnOffColor;
                            horizoltalConveyor_GTP1.pnPusherLeftB.BackColor = turnOffColor;
                            horizoltalConveyor_GTP1.pnPusherLeftC.BackColor = turnOffColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_03B.BackColor = turnOffColor;
                        }
                        break;
                    case 3:
                        if (options == 0)
                        {
                            verticalConveyor_GTP1_Left.pnStopperTop.BackColor = turnOffColor;
                        }
                        else if (options == 1)
                        {
                            mainConveyor.pnPusher04a.BackColor = turnOffColor;
                            mainConveyor.pnPusher04b.BackColor = turnOffColor;
                            mainConveyor.pnPusher04c.BackColor = turnOffColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_05.BackColor = turnOffColor;
                        }
                        break;
                    case 4:
                        if (options == 0)
                        {
                            mainConveyor.pnStopperLeft.BackColor = turnOffColor;
                        }
                        else if (options == 1)
                        {
                            mainConveyor.pnPusher05a.BackColor = turnOffColor;
                            mainConveyor.pnPusher05b.BackColor = turnOffColor;
                            mainConveyor.pnPusher05c.BackColor = turnOffColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_07A.BackColor = turnOffColor;
                        }
                        break;
                    case 5:
                        if (options == 0)
                        {
                            verticalConveyor_GTP2_Right.pnStopperBottom.BackColor = turnOffColor;
                        }
                        else if (options == 1)
                        {
                            horizoltalConveyor_GTP2.pnPusherRightA.BackColor = turnOffColor;
                            horizoltalConveyor_GTP2.pnPusherRightB.BackColor = turnOffColor;
                            horizoltalConveyor_GTP2.pnPusherRightC.BackColor = turnOffColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_07B.BackColor = turnOffColor;
                        }
                        break;
                    case 6:
                        if (options == 0)
                        {
                            horizoltalConveyor_GTP2.pnStopper.BackColor = turnOffColor;
                        }
                        else if (options == 1)
                        {
                            horizoltalConveyor_GTP2.pnPusherLeftA.BackColor = turnOffColor;
                            horizoltalConveyor_GTP2.pnPusherLeftB.BackColor = turnOffColor;
                            horizoltalConveyor_GTP2.pnPusherLeftC.BackColor = turnOffColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_01sc.BackColor = turnOffColor;
                        }
                        break;
                    case 7:
                        if (options == 0)
                        {
                            verticalConveyor_GTP2_Left.pnStopperTop.BackColor = turnOffColor;
                        }
                        else if (options == 1)
                        {
                            mainConveyor.pnPusher08a.BackColor = turnOffColor;
                            mainConveyor.pnPusher08b.BackColor = turnOffColor;
                            mainConveyor.pnPusher08c.BackColor = turnOffColor;
                        }
                        else if (options == 2)
                        {
                            pnSs_02sc.BackColor = turnOffColor;
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        private void RemoveContainer()
        {
            if (grapher == null) return;
            if (grapher.Containers[8, 13].Count > 0)
            {
                Container containerItem = grapher.Containers[8, 13][0];
                grapher.Containers[8, 13].Remove(containerItem);
            }
        }

        private void CheckSystemAlready()
        {
            if (WesSocket.Status != StatusEnum.Connected || Scanner01Socket.Status != StatusEnum.Connected || Scanner02Socket.Status != StatusEnum.Connected || Plc.Status != StatusEnum.Connected)
                OnShowReady("The systems is not ready.", Color.Orange);
            else
                OnShowReady("Ready", Color.LimeGreen);
        }

        #endregion


        #region Effects

        public static bool note_gtp_01 = false;
        public static bool note_gtp_02 = false;
        public static bool note_end = false;
        public static bool note_ng = false;
        public static bool note_complete = false;
        public static bool note_advance = false;
        int count_gtp_01 = 0, count_gtp_02 = 0, count_ng = 0, count_end = 0, count_advance = 0, count_complete = 0;
        private void TmrDisplayNote_Tick(object? sender, EventArgs e)
        {
            if (grapher == null) return;

            if (grapher.GTP_01_STATUS == StatusEnum.Ready)
            {
                btnGTP_01_Status.Text = StatusEnum.Ready.ToString();
                btnGTP_01_Status.ForeColor = Color.White;
                btnGTP_01_Status.BackColor = Color.LightSkyBlue;
            }
            else if (grapher.GTP_01_STATUS == StatusEnum.Busy)
            {
                btnGTP_01_Status.Text = StatusEnum.Busy.ToString();
                btnGTP_01_Status.ForeColor = Color.White;
                btnGTP_01_Status.BackColor = Color.LightSalmon;
            }

            if (grapher.GTP_02_STATUS == StatusEnum.Ready)
            {
                btnGTP_02_Status.Text = StatusEnum.Ready.ToString();
                btnGTP_02_Status.ForeColor = Color.White;
                btnGTP_02_Status.BackColor = Color.LightSkyBlue;
            }
            else if (grapher.GTP_02_STATUS == StatusEnum.Busy)
            {
                btnGTP_02_Status.Text = StatusEnum.Busy.ToString();
                btnGTP_02_Status.ForeColor = Color.White;
                btnGTP_02_Status.BackColor = Color.LightSalmon;
            }

            if (note_gtp_01)
            {
                if (pnGTP01.BackColor == Color.LightGray) pnGTP01.BackColor = Color.DodgerBlue;
                else pnGTP01.BackColor = Color.LightGray;

                count_gtp_01++;
                if (count_gtp_01 >= 5)
                {
                    note_gtp_01 = false;
                    count_gtp_01 = 0;
                }
            }
            else pnGTP01.BackColor = Color.DodgerBlue;

            if (note_gtp_02)
            {
                if (pnGTP02.BackColor == Color.LightGray) pnGTP02.BackColor = Color.RoyalBlue;
                else pnGTP02.BackColor = Color.LightGray;

                count_gtp_02++;
                if (count_gtp_02 >= 5)
                {
                    note_gtp_02 = false;
                    count_gtp_02 = 0;
                }
            }
            else pnGTP02.BackColor = Color.RoyalBlue;

            if (note_ng)
            {
                if (pnNG.BackColor == Color.LightGray) pnNG.BackColor = Color.Red;
                else pnNG.BackColor = Color.LightGray;

                count_ng++;
                if (count_ng >= 5)
                {
                    note_ng = false;
                    count_ng = 0;
                }
            }
            else pnNG.BackColor = Color.Red;

            if (note_end)
            {
                if (pnEND.BackColor == Color.LightGray) pnEND.BackColor = Color.LightSalmon;
                else pnEND.BackColor = Color.LightGray;

                count_end++;
                if (count_end >= 5)
                {
                    note_end = false;
                    count_end = 0;
                }
            }
            else pnEND.BackColor = Color.LightSalmon;

            if (note_advance)
            {
                if (pnADVANCE.BackColor == Color.LightGray) pnADVANCE.BackColor = Color.Yellow;
                else pnADVANCE.BackColor = Color.LightGray;

                count_advance++;
                if (count_advance >= 5)
                {
                    note_advance = false;
                    count_advance = 0;
                }
            }
            else pnADVANCE.BackColor = Color.Yellow;

            if (note_complete)
            {
                if (pnCOMPLETE.BackColor == Color.LightGray) pnCOMPLETE.BackColor = Color.LimeGreen;
                else pnCOMPLETE.BackColor = Color.LightGray;

                count_complete++;
                if (count_complete >= 5)
                {
                    note_complete = false;
                    count_complete = 0;
                }
            }
            else pnCOMPLETE.BackColor = Color.LimeGreen;
        }

        #endregion


    }
}
