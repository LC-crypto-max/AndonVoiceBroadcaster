using System.Drawing;
using System.Windows.Forms;

namespace AndonVoiceBroadcaster.Forms
{
    partial class SettingsForm
    {
        private Label captionLabel1;
        private Label captionLabel2;
        private Label captionLabel3;
        private Label captionLabel4;
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.captionLabel1 = new System.Windows.Forms.Label();
            this.captionLabel2 = new System.Windows.Forms.Label();
            this.captionLabel3 = new System.Windows.Forms.Label();
            this.captionLabel4 = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.TableLayoutPanel();
            this._mesBaseUrlLabel = new System.Windows.Forms.Label();
            this._mesBaseUrl = new System.Windows.Forms.TextBox();
            this._usernameLabel = new System.Windows.Forms.Label();
            this._username = new System.Windows.Forms.TextBox();
            this._passwordLabel = new System.Windows.Forms.Label();
            this._password = new System.Windows.Forms.TextBox();
            this._terminalCodeLabel = new System.Windows.Forms.Label();
            this._terminalCode = new System.Windows.Forms.TextBox();
            this.workshopGroup = new System.Windows.Forms.GroupBox();
            this.workshopTable = new System.Windows.Forms.TableLayoutPanel();
            this.factoryPanel = new System.Windows.Forms.TableLayoutPanel();
            this._factory = new System.Windows.Forms.ComboBox();
            this._availableWorkshops = new System.Windows.Forms.ListBox();
            this._selectedWorkshops = new System.Windows.Forms.ListBox();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._addWorkshopButton = new System.Windows.Forms.Button();
            this._removeWorkshopButton = new System.Windows.Forms.Button();
            this._addAllButton = new System.Windows.Forms.Button();
            this._removeAllButton = new System.Windows.Forms.Button();
            this._availableWorkshopCount = new System.Windows.Forms.Label();
            this._refreshWorkshopButton = new System.Windows.Forms.Button();
            this._selectedWorkshopCount = new System.Windows.Forms.Label();
            this.parameterGroup = new System.Windows.Forms.GroupBox();
            this.parameterTable = new System.Windows.Forms.TableLayoutPanel();
            this._pollSecondsLabel = new System.Windows.Forms.Label();
            this._pollSeconds = new System.Windows.Forms.NumericUpDown();
            this._retrySecondsLabel = new System.Windows.Forms.Label();
            this._retrySeconds = new System.Windows.Forms.NumericUpDown();
            this._ttsRateLabel = new System.Windows.Forms.Label();
            this._ttsRate = new System.Windows.Forms.NumericUpDown();
            this._ttsVolumeLabel = new System.Windows.Forms.Label();
            this._ttsVolume = new System.Windows.Forms.NumericUpDown();
            this._testSpeechTextLabel = new System.Windows.Forms.Label();
            this._testSpeechText = new System.Windows.Forms.TextBox();
            this._broadcastIntervalLabel = new System.Windows.Forms.Label();
            this._broadcastInterval = new System.Windows.Forms.NumericUpDown();
            this._workStartTimeLabel = new System.Windows.Forms.Label();
            this._workStartTime = new System.Windows.Forms.DateTimePicker();
            this._workEndTimeLabel = new System.Windows.Forms.Label();
            this._workEndTime = new System.Windows.Forms.DateTimePicker();
            this.options = new System.Windows.Forms.FlowLayoutPanel();
            this._startWithWindows = new System.Windows.Forms.CheckBox();
            this._startMinimized = new System.Windows.Forms.CheckBox();
            this.footerLabel = new System.Windows.Forms.Label();
            this.buttons = new System.Windows.Forms.FlowLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.panel.SuspendLayout();
            this.workshopGroup.SuspendLayout();
            this.workshopTable.SuspendLayout();
            this.factoryPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.parameterGroup.SuspendLayout();
            this.parameterTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pollSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._retrySeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._ttsRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._ttsVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._broadcastInterval)).BeginInit();
            this.options.SuspendLayout();
            this.buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // captionLabel1
            // 
            this.captionLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captionLabel1.Location = new System.Drawing.Point(4, 0);
            this.captionLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.captionLabel1.Name = "captionLabel1";
            this.captionLabel1.Size = new System.Drawing.Size(112, 33);
            this.captionLabel1.TabIndex = 0;
            this.captionLabel1.Text = "选择工厂";
            this.captionLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // captionLabel2
            // 
            this.captionLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captionLabel2.Location = new System.Drawing.Point(4, 39);
            this.captionLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.captionLabel2.Name = "captionLabel2";
            this.captionLabel2.Size = new System.Drawing.Size(479, 32);
            this.captionLabel2.TabIndex = 1;
            this.captionLabel2.Text = "可用车间";
            this.captionLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // captionLabel3
            // 
            this.captionLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captionLabel3.Location = new System.Drawing.Point(640, 39);
            this.captionLabel3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.captionLabel3.Name = "captionLabel3";
            this.captionLabel3.Size = new System.Drawing.Size(480, 32);
            this.captionLabel3.TabIndex = 2;
            this.captionLabel3.Text = "已选车间";
            this.captionLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // captionLabel4
            // 
            this.captionLabel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captionLabel4.Location = new System.Drawing.Point(25, 672);
            this.captionLabel4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.captionLabel4.Name = "captionLabel4";
            this.captionLabel4.Size = new System.Drawing.Size(192, 67);
            this.captionLabel4.TabIndex = 10;
            this.captionLabel4.Text = "启动选项";
            this.captionLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel
            // 
            this.panel.ColumnCount = 2;
            this.panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel.Controls.Add(this._mesBaseUrlLabel, 0, 0);
            this.panel.Controls.Add(this._mesBaseUrl, 1, 0);
            this.panel.Controls.Add(this._usernameLabel, 0, 1);
            this.panel.Controls.Add(this._username, 1, 1);
            this.panel.Controls.Add(this._passwordLabel, 0, 2);
            this.panel.Controls.Add(this._password, 1, 2);
            this.panel.Controls.Add(this._terminalCodeLabel, 0, 3);
            this.panel.Controls.Add(this._terminalCode, 1, 3);
            this.panel.Controls.Add(this.workshopGroup, 0, 4);
            this.panel.Controls.Add(this.parameterGroup, 0, 5);
            this.panel.Controls.Add(this.captionLabel4, 0, 6);
            this.panel.Controls.Add(this.options, 1, 6);
            this.panel.Controls.Add(this.footerLabel, 0, 7);
            this.panel.Controls.Add(this.buttons, 1, 7);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel.Name = "panel";
            this.panel.Padding = new System.Windows.Forms.Padding(21, 18, 21, 18);
            this.panel.RowCount = 8;
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.panel.Size = new System.Drawing.Size(1200, 808);
            this.panel.TabIndex = 0;
            // 
            // _mesBaseUrlLabel
            // 
            this._mesBaseUrlLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mesBaseUrlLabel.Location = new System.Drawing.Point(25, 18);
            this._mesBaseUrlLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._mesBaseUrlLabel.Name = "_mesBaseUrlLabel";
            this._mesBaseUrlLabel.Size = new System.Drawing.Size(192, 39);
            this._mesBaseUrlLabel.TabIndex = 0;
            this._mesBaseUrlLabel.Text = "MES 服务地址";
            this._mesBaseUrlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _mesBaseUrl
            // 
            this._mesBaseUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mesBaseUrl.Location = new System.Drawing.Point(225, 24);
            this._mesBaseUrl.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._mesBaseUrl.Name = "_mesBaseUrl";
            this._mesBaseUrl.Size = new System.Drawing.Size(950, 25);
            this._mesBaseUrl.TabIndex = 1;
            // 
            // _usernameLabel
            // 
            this._usernameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._usernameLabel.Location = new System.Drawing.Point(25, 57);
            this._usernameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._usernameLabel.Name = "_usernameLabel";
            this._usernameLabel.Size = new System.Drawing.Size(192, 39);
            this._usernameLabel.TabIndex = 2;
            this._usernameLabel.Text = "登录账号";
            this._usernameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _username
            // 
            this._username.Dock = System.Windows.Forms.DockStyle.Fill;
            this._username.Location = new System.Drawing.Point(225, 63);
            this._username.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._username.Name = "_username";
            this._username.Size = new System.Drawing.Size(950, 25);
            this._username.TabIndex = 3;
            // 
            // _passwordLabel
            // 
            this._passwordLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._passwordLabel.Location = new System.Drawing.Point(25, 96);
            this._passwordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._passwordLabel.Name = "_passwordLabel";
            this._passwordLabel.Size = new System.Drawing.Size(192, 39);
            this._passwordLabel.TabIndex = 4;
            this._passwordLabel.Text = "登录密码";
            this._passwordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _password
            // 
            this._password.Dock = System.Windows.Forms.DockStyle.Fill;
            this._password.Location = new System.Drawing.Point(225, 102);
            this._password.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._password.Name = "_password";
            this._password.Size = new System.Drawing.Size(950, 25);
            this._password.TabIndex = 5;
            this._password.UseSystemPasswordChar = true;
            // 
            // _terminalCodeLabel
            // 
            this._terminalCodeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._terminalCodeLabel.Location = new System.Drawing.Point(25, 135);
            this._terminalCodeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._terminalCodeLabel.Name = "_terminalCodeLabel";
            this._terminalCodeLabel.Size = new System.Drawing.Size(192, 39);
            this._terminalCodeLabel.TabIndex = 6;
            this._terminalCodeLabel.Text = "终端编号";
            this._terminalCodeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _terminalCode
            // 
            this._terminalCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this._terminalCode.Location = new System.Drawing.Point(225, 141);
            this._terminalCode.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._terminalCode.Name = "_terminalCode";
            this._terminalCode.Size = new System.Drawing.Size(950, 25);
            this._terminalCode.TabIndex = 7;
            // 
            // workshopGroup
            // 
            this.panel.SetColumnSpan(this.workshopGroup, 2);
            this.workshopGroup.Controls.Add(this.workshopTable);
            this.workshopGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workshopGroup.Location = new System.Drawing.Point(25, 183);
            this.workshopGroup.Margin = new System.Windows.Forms.Padding(4, 9, 4, 9);
            this.workshopGroup.Name = "workshopGroup";
            this.workshopGroup.Padding = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this.workshopGroup.Size = new System.Drawing.Size(1150, 307);
            this.workshopGroup.TabIndex = 8;
            this.workshopGroup.TabStop = false;
            this.workshopGroup.Text = "车间播报范围（未选择时默认播报全部车间）";
            // 
            // workshopTable
            // 
            this.workshopTable.ColumnCount = 3;
            this.workshopTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.workshopTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 149F));
            this.workshopTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.workshopTable.Controls.Add(this.factoryPanel, 0, 0);
            this.workshopTable.Controls.Add(this.captionLabel2, 0, 1);
            this.workshopTable.Controls.Add(this.captionLabel3, 2, 1);
            this.workshopTable.Controls.Add(this._availableWorkshops, 0, 2);
            this.workshopTable.Controls.Add(this._selectedWorkshops, 2, 2);
            this.workshopTable.Controls.Add(this.buttonPanel, 1, 1);
            this.workshopTable.Controls.Add(this._availableWorkshopCount, 0, 3);
            this.workshopTable.Controls.Add(this._refreshWorkshopButton, 1, 3);
            this.workshopTable.Controls.Add(this._selectedWorkshopCount, 2, 3);
            this.workshopTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workshopTable.Location = new System.Drawing.Point(13, 30);
            this.workshopTable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.workshopTable.Name = "workshopTable";
            this.workshopTable.RowCount = 4;
            this.workshopTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.workshopTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.workshopTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.workshopTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.workshopTable.Size = new System.Drawing.Size(1124, 265);
            this.workshopTable.TabIndex = 0;
            // 
            // factoryPanel
            // 
            this.factoryPanel.ColumnCount = 2;
            this.workshopTable.SetColumnSpan(this.factoryPanel, 3);
            this.factoryPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.factoryPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.factoryPanel.Controls.Add(this.captionLabel1, 0, 0);
            this.factoryPanel.Controls.Add(this._factory, 1, 0);
            this.factoryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.factoryPanel.Location = new System.Drawing.Point(4, 3);
            this.factoryPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.factoryPanel.Name = "factoryPanel";
            this.factoryPanel.RowCount = 1;
            this.factoryPanel.Size = new System.Drawing.Size(1116, 33);
            this.factoryPanel.TabIndex = 0;
            // 
            // _factory
            // 
            this._factory.Dock = System.Windows.Forms.DockStyle.Fill;
            this._factory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._factory.Location = new System.Drawing.Point(120, 3);
            this._factory.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this._factory.Name = "_factory";
            this._factory.Size = new System.Drawing.Size(996, 23);
            this._factory.TabIndex = 1;
            this._factory.SelectedIndexChanged += new System.EventHandler(this.FactoryOnSelectedIndexChanged);
            // 
            // _availableWorkshops
            // 
            this._availableWorkshops.Dock = System.Windows.Forms.DockStyle.Fill;
            this._availableWorkshops.HorizontalScrollbar = true;
            this._availableWorkshops.IntegralHeight = false;
            this._availableWorkshops.ItemHeight = 15;
            this._availableWorkshops.Location = new System.Drawing.Point(0, 73);
            this._availableWorkshops.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this._availableWorkshops.Name = "_availableWorkshops";
            this._availableWorkshops.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._availableWorkshops.Size = new System.Drawing.Size(487, 148);
            this._availableWorkshops.TabIndex = 3;
            this._availableWorkshops.SelectedIndexChanged += new System.EventHandler(this.WorkshopSelectionChanged);
            this._availableWorkshops.DoubleClick += new System.EventHandler(this.AddWorkshopButtonOnClick);
            // 
            // _selectedWorkshops
            // 
            this._selectedWorkshops.Dock = System.Windows.Forms.DockStyle.Fill;
            this._selectedWorkshops.HorizontalScrollbar = true;
            this._selectedWorkshops.IntegralHeight = false;
            this._selectedWorkshops.ItemHeight = 15;
            this._selectedWorkshops.Location = new System.Drawing.Point(636, 73);
            this._selectedWorkshops.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this._selectedWorkshops.Name = "_selectedWorkshops";
            this._selectedWorkshops.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._selectedWorkshops.Size = new System.Drawing.Size(488, 148);
            this._selectedWorkshops.TabIndex = 4;
            this._selectedWorkshops.SelectedIndexChanged += new System.EventHandler(this.WorkshopSelectionChanged);
            this._selectedWorkshops.DoubleClick += new System.EventHandler(this.RemoveWorkshopButtonOnClick);
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this._addWorkshopButton);
            this.buttonPanel.Controls.Add(this._removeWorkshopButton);
            this.buttonPanel.Controls.Add(this._addAllButton);
            this.buttonPanel.Controls.Add(this._removeAllButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.buttonPanel.Location = new System.Drawing.Point(491, 42);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(11, 5, 11, 0);
            this.workshopTable.SetRowSpan(this.buttonPanel, 2);
            this.buttonPanel.Size = new System.Drawing.Size(141, 178);
            this.buttonPanel.TabIndex = 5;
            this.buttonPanel.WrapContents = false;
            // 
            // _addWorkshopButton
            // 
            this._addWorkshopButton.Location = new System.Drawing.Point(11, 5);
            this._addWorkshopButton.Margin = new System.Windows.Forms.Padding(0);
            this._addWorkshopButton.Name = "_addWorkshopButton";
            this._addWorkshopButton.Size = new System.Drawing.Size(120, 25);
            this._addWorkshopButton.TabIndex = 0;
            this._addWorkshopButton.Text = "添加 >";
            this._addWorkshopButton.Click += new System.EventHandler(this.AddWorkshopButtonOnClick);
            // 
            // _removeWorkshopButton
            // 
            this._removeWorkshopButton.Location = new System.Drawing.Point(11, 30);
            this._removeWorkshopButton.Margin = new System.Windows.Forms.Padding(0);
            this._removeWorkshopButton.Name = "_removeWorkshopButton";
            this._removeWorkshopButton.Size = new System.Drawing.Size(120, 25);
            this._removeWorkshopButton.TabIndex = 1;
            this._removeWorkshopButton.Text = "< 移除";
            this._removeWorkshopButton.Click += new System.EventHandler(this.RemoveWorkshopButtonOnClick);
            // 
            // _addAllButton
            // 
            this._addAllButton.Location = new System.Drawing.Point(11, 55);
            this._addAllButton.Margin = new System.Windows.Forms.Padding(0);
            this._addAllButton.Name = "_addAllButton";
            this._addAllButton.Size = new System.Drawing.Size(120, 25);
            this._addAllButton.TabIndex = 2;
            this._addAllButton.Text = "全部 >>";
            this._addAllButton.Click += new System.EventHandler(this.AddAllButtonOnClick);
            // 
            // _removeAllButton
            // 
            this._removeAllButton.Location = new System.Drawing.Point(11, 80);
            this._removeAllButton.Margin = new System.Windows.Forms.Padding(0);
            this._removeAllButton.Name = "_removeAllButton";
            this._removeAllButton.Size = new System.Drawing.Size(120, 25);
            this._removeAllButton.TabIndex = 3;
            this._removeAllButton.Text = "<< 清空";
            this._removeAllButton.Click += new System.EventHandler(this.RemoveAllButtonOnClick);
            // 
            // _availableWorkshopCount
            // 
            this._availableWorkshopCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this._availableWorkshopCount.Location = new System.Drawing.Point(4, 223);
            this._availableWorkshopCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._availableWorkshopCount.Name = "_availableWorkshopCount";
            this._availableWorkshopCount.Size = new System.Drawing.Size(479, 42);
            this._availableWorkshopCount.TabIndex = 6;
            this._availableWorkshopCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _refreshWorkshopButton
            // 
            this._refreshWorkshopButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._refreshWorkshopButton.Location = new System.Drawing.Point(498, 226);
            this._refreshWorkshopButton.Margin = new System.Windows.Forms.Padding(11, 3, 11, 3);
            this._refreshWorkshopButton.Name = "_refreshWorkshopButton";
            this._refreshWorkshopButton.Size = new System.Drawing.Size(127, 36);
            this._refreshWorkshopButton.TabIndex = 7;
            this._refreshWorkshopButton.Text = "刷新列表";
            this._refreshWorkshopButton.Click += new System.EventHandler(this.RefreshWorkshopButtonOnClick);
            // 
            // _selectedWorkshopCount
            // 
            this._selectedWorkshopCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this._selectedWorkshopCount.Location = new System.Drawing.Point(640, 223);
            this._selectedWorkshopCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._selectedWorkshopCount.Name = "_selectedWorkshopCount";
            this._selectedWorkshopCount.Size = new System.Drawing.Size(480, 42);
            this._selectedWorkshopCount.TabIndex = 8;
            this._selectedWorkshopCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // parameterGroup
            // 
            this.panel.SetColumnSpan(this.parameterGroup, 2);
            this.parameterGroup.Controls.Add(this.parameterTable);
            this.parameterGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameterGroup.Location = new System.Drawing.Point(25, 499);
            this.parameterGroup.Margin = new System.Windows.Forms.Padding(4, 0, 4, 9);
            this.parameterGroup.Name = "parameterGroup";
            this.parameterGroup.Padding = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this.parameterGroup.Size = new System.Drawing.Size(1150, 164);
            this.parameterGroup.TabIndex = 9;
            this.parameterGroup.TabStop = false;
            this.parameterGroup.Text = "轮询与播报参数";
            // 
            // parameterTable
            // 
            this.parameterTable.ColumnCount = 4;
            this.parameterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this.parameterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.parameterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this.parameterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.parameterTable.Controls.Add(this._pollSecondsLabel, 0, 0);
            this.parameterTable.Controls.Add(this._pollSeconds, 1, 0);
            this.parameterTable.Controls.Add(this._retrySecondsLabel, 2, 0);
            this.parameterTable.Controls.Add(this._retrySeconds, 3, 0);
            this.parameterTable.Controls.Add(this._ttsRateLabel, 0, 1);
            this.parameterTable.Controls.Add(this._ttsRate, 1, 1);
            this.parameterTable.Controls.Add(this._ttsVolumeLabel, 2, 1);
            this.parameterTable.Controls.Add(this._ttsVolume, 3, 1);
            this.parameterTable.Controls.Add(this._testSpeechTextLabel, 0, 2);
            this.parameterTable.Controls.Add(this._testSpeechText, 1, 2);
            this.parameterTable.Controls.Add(this._broadcastIntervalLabel, 2, 2);
            this.parameterTable.Controls.Add(this._broadcastInterval, 3, 2);
            this.parameterTable.Controls.Add(this._workStartTimeLabel, 0, 3);
            this.parameterTable.Controls.Add(this._workStartTime, 1, 3);
            this.parameterTable.Controls.Add(this._workEndTimeLabel, 2, 3);
            this.parameterTable.Controls.Add(this._workEndTime, 3, 3);
            this.parameterTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameterTable.Location = new System.Drawing.Point(13, 30);
            this.parameterTable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.parameterTable.Name = "parameterTable";
            this.parameterTable.RowCount = 4;
            this.parameterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.parameterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.parameterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.parameterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.parameterTable.Size = new System.Drawing.Size(1124, 122);
            this.parameterTable.TabIndex = 0;
            // 
            // _pollSecondsLabel
            // 
            this._pollSecondsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pollSecondsLabel.Location = new System.Drawing.Point(4, 0);
            this._pollSecondsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._pollSecondsLabel.Name = "_pollSecondsLabel";
            this._pollSecondsLabel.Size = new System.Drawing.Size(165, 30);
            this._pollSecondsLabel.TabIndex = 0;
            this._pollSecondsLabel.Text = "查询间隔（秒）";
            this._pollSecondsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _pollSeconds
            // 
            this._pollSeconds.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pollSeconds.Location = new System.Drawing.Point(177, 6);
            this._pollSeconds.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._pollSeconds.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this._pollSeconds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._pollSeconds.Name = "_pollSeconds";
            this._pollSeconds.Size = new System.Drawing.Size(381, 25);
            this._pollSeconds.TabIndex = 1;
            this._pollSeconds.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // _retrySecondsLabel
            // 
            this._retrySecondsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._retrySecondsLabel.Location = new System.Drawing.Point(566, 0);
            this._retrySecondsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._retrySecondsLabel.Name = "_retrySecondsLabel";
            this._retrySecondsLabel.Size = new System.Drawing.Size(165, 30);
            this._retrySecondsLabel.TabIndex = 2;
            this._retrySecondsLabel.Text = "网络重试（秒）";
            this._retrySecondsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _retrySeconds
            // 
            this._retrySeconds.Dock = System.Windows.Forms.DockStyle.Fill;
            this._retrySeconds.Location = new System.Drawing.Point(739, 6);
            this._retrySeconds.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._retrySeconds.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this._retrySeconds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._retrySeconds.Name = "_retrySeconds";
            this._retrySeconds.Size = new System.Drawing.Size(381, 25);
            this._retrySeconds.TabIndex = 3;
            this._retrySeconds.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // _ttsRateLabel
            // 
            this._ttsRateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ttsRateLabel.Location = new System.Drawing.Point(4, 30);
            this._ttsRateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._ttsRateLabel.Name = "_ttsRateLabel";
            this._ttsRateLabel.Size = new System.Drawing.Size(165, 30);
            this._ttsRateLabel.TabIndex = 4;
            this._ttsRateLabel.Text = "TTS 语速";
            this._ttsRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _ttsRate
            // 
            this._ttsRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ttsRate.Location = new System.Drawing.Point(177, 36);
            this._ttsRate.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._ttsRate.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this._ttsRate.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this._ttsRate.Name = "_ttsRate";
            this._ttsRate.Size = new System.Drawing.Size(381, 25);
            this._ttsRate.TabIndex = 5;
            // 
            // _ttsVolumeLabel
            // 
            this._ttsVolumeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ttsVolumeLabel.Location = new System.Drawing.Point(566, 30);
            this._ttsVolumeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._ttsVolumeLabel.Name = "_ttsVolumeLabel";
            this._ttsVolumeLabel.Size = new System.Drawing.Size(165, 30);
            this._ttsVolumeLabel.TabIndex = 6;
            this._ttsVolumeLabel.Text = "TTS 音量";
            this._ttsVolumeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _ttsVolume
            // 
            this._ttsVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ttsVolume.Location = new System.Drawing.Point(739, 36);
            this._ttsVolume.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._ttsVolume.Name = "_ttsVolume";
            this._ttsVolume.Size = new System.Drawing.Size(381, 25);
            this._ttsVolume.TabIndex = 7;
            this._ttsVolume.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // _testSpeechTextLabel
            // 
            this._testSpeechTextLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._testSpeechTextLabel.Location = new System.Drawing.Point(4, 60);
            this._testSpeechTextLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._testSpeechTextLabel.Name = "_testSpeechTextLabel";
            this._testSpeechTextLabel.Size = new System.Drawing.Size(165, 30);
            this._testSpeechTextLabel.TabIndex = 8;
            this._testSpeechTextLabel.Text = "本地测试文本";
            this._testSpeechTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _testSpeechText
            // 
            this._testSpeechText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._testSpeechText.Location = new System.Drawing.Point(177, 66);
            this._testSpeechText.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._testSpeechText.Name = "_testSpeechText";
            this._testSpeechText.Size = new System.Drawing.Size(381, 25);
            this._testSpeechText.TabIndex = 9;
            // 
            // _broadcastIntervalLabel
            // 
            this._broadcastIntervalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._broadcastIntervalLabel.Location = new System.Drawing.Point(566, 60);
            this._broadcastIntervalLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._broadcastIntervalLabel.Name = "_broadcastIntervalLabel";
            this._broadcastIntervalLabel.Size = new System.Drawing.Size(165, 30);
            this._broadcastIntervalLabel.TabIndex = 10;
            this._broadcastIntervalLabel.Text = "播报间隔（秒）";
            this._broadcastIntervalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _broadcastInterval
            // 
            this._broadcastInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this._broadcastInterval.Location = new System.Drawing.Point(739, 66);
            this._broadcastInterval.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._broadcastInterval.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this._broadcastInterval.Name = "_broadcastInterval";
            this._broadcastInterval.Size = new System.Drawing.Size(381, 25);
            this._broadcastInterval.TabIndex = 11;
            this._broadcastInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _workStartTimeLabel
            // 
            this._workStartTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workStartTimeLabel.Location = new System.Drawing.Point(4, 90);
            this._workStartTimeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._workStartTimeLabel.Name = "_workStartTimeLabel";
            this._workStartTimeLabel.Size = new System.Drawing.Size(165, 32);
            this._workStartTimeLabel.TabIndex = 12;
            this._workStartTimeLabel.Text = "上班开始时间";
            this._workStartTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _workStartTime
            // 
            this._workStartTime.CustomFormat = "HH:mm";
            this._workStartTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._workStartTime.Location = new System.Drawing.Point(177, 96);
            this._workStartTime.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._workStartTime.Name = "_workStartTime";
            this._workStartTime.ShowUpDown = true;
            this._workStartTime.Size = new System.Drawing.Size(381, 25);
            this._workStartTime.TabIndex = 13;
            // 
            // _workEndTimeLabel
            // 
            this._workEndTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workEndTimeLabel.Location = new System.Drawing.Point(566, 90);
            this._workEndTimeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._workEndTimeLabel.Name = "_workEndTimeLabel";
            this._workEndTimeLabel.Size = new System.Drawing.Size(165, 32);
            this._workEndTimeLabel.TabIndex = 14;
            this._workEndTimeLabel.Text = "下班停止时间";
            this._workEndTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _workEndTime
            // 
            this._workEndTime.CustomFormat = "HH:mm";
            this._workEndTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._workEndTime.Location = new System.Drawing.Point(739, 96);
            this._workEndTime.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this._workEndTime.Name = "_workEndTime";
            this._workEndTime.ShowUpDown = true;
            this._workEndTime.Size = new System.Drawing.Size(381, 25);
            this._workEndTime.TabIndex = 15;
            // 
            // options
            // 
            this.options.AutoSize = true;
            this.options.Controls.Add(this._startWithWindows);
            this.options.Controls.Add(this._startMinimized);
            this.options.Dock = System.Windows.Forms.DockStyle.Fill;
            this.options.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.options.Location = new System.Drawing.Point(225, 675);
            this.options.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.options.Name = "options";
            this.options.Size = new System.Drawing.Size(950, 61);
            this.options.TabIndex = 11;
            // 
            // _startWithWindows
            // 
            this._startWithWindows.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._startWithWindows.AutoSize = true;
            this._startWithWindows.Location = new System.Drawing.Point(4, 3);
            this._startWithWindows.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._startWithWindows.Name = "_startWithWindows";
            this._startWithWindows.Size = new System.Drawing.Size(266, 19);
            this._startWithWindows.TabIndex = 0;
            this._startWithWindows.Text = "当前 Windows 用户登录后自动启动";
            // 
            // _startMinimized
            // 
            this._startMinimized.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._startMinimized.AutoSize = true;
            this._startMinimized.Location = new System.Drawing.Point(4, 28);
            this._startMinimized.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._startMinimized.Name = "_startMinimized";
            this._startMinimized.Size = new System.Drawing.Size(194, 19);
            this._startMinimized.TabIndex = 1;
            this._startMinimized.Text = "启动后最小化到系统托盘";
            // 
            // footerLabel
            // 
            this.footerLabel.Location = new System.Drawing.Point(25, 739);
            this.footerLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.footerLabel.Name = "footerLabel";
            this.footerLabel.Size = new System.Drawing.Size(133, 27);
            this.footerLabel.TabIndex = 12;
            // 
            // buttons
            // 
            this.buttons.Controls.Add(this.saveButton);
            this.buttons.Controls.Add(this.cancelButton);
            this.buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttons.Location = new System.Drawing.Point(225, 742);
            this.buttons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttons.Name = "buttons";
            this.buttons.Size = new System.Drawing.Size(950, 45);
            this.buttons.TabIndex = 13;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(829, 3);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(117, 27);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "保存";
            this.saveButton.Click += new System.EventHandler(this.SaveButtonOnClick);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(704, 3);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(117, 27);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "取消";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(1200, 808);
            this.Controls.Add(this.panel);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1087, 777);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "安灯语音播报 - 系统设置";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.workshopGroup.ResumeLayout(false);
            this.workshopTable.ResumeLayout(false);
            this.factoryPanel.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this.parameterGroup.ResumeLayout(false);
            this.parameterTable.ResumeLayout(false);
            this.parameterTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pollSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._retrySeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._ttsRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._ttsVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._broadcastInterval)).EndInit();
            this.options.ResumeLayout(false);
            this.options.PerformLayout();
            this.buttons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private Label _mesBaseUrlLabel;
        private TextBox _mesBaseUrl;
        private Label _usernameLabel;
        private TextBox _username;
        private Label _passwordLabel;
        private TextBox _password;
        private Label _terminalCodeLabel;
        private TextBox _terminalCode;
        private Label _pollSecondsLabel;
        private NumericUpDown _pollSeconds;
        private Label _retrySecondsLabel;
        private NumericUpDown _retrySeconds;
        private Label _ttsRateLabel;
        private NumericUpDown _ttsRate;
        private Label _ttsVolumeLabel;
        private NumericUpDown _ttsVolume;
        private Label _testSpeechTextLabel;
        private TextBox _testSpeechText;
        private Label _broadcastIntervalLabel;
        private NumericUpDown _broadcastInterval;
        private Label _workStartTimeLabel;
        private DateTimePicker _workStartTime;
        private Label _workEndTimeLabel;
        private DateTimePicker _workEndTime;
        private ListBox _availableWorkshops;
        private ListBox _selectedWorkshops;
        private TableLayoutPanel panel;
        private GroupBox workshopGroup;
        private TableLayoutPanel workshopTable;
        private TableLayoutPanel factoryPanel;
        private ComboBox _factory;
        private FlowLayoutPanel buttonPanel;
        private Button _addWorkshopButton;
        private Button _removeWorkshopButton;
        private Button _addAllButton;
        private Button _removeAllButton;
        private Label _availableWorkshopCount;
        private Label _selectedWorkshopCount;
        private Button _refreshWorkshopButton;
        private GroupBox parameterGroup;
        private TableLayoutPanel parameterTable;
        private CheckBox _startWithWindows;
        private CheckBox _startMinimized;
        private FlowLayoutPanel options;
        private FlowLayoutPanel buttons;
        private Button saveButton;
        private Button cancelButton;
        private Label footerLabel;
    }
}
