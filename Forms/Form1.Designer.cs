using System.Drawing;
using System.Windows.Forms;

namespace AndonVoiceBroadcaster.Forms
{
    partial class Form1
    {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.topPanel = new System.Windows.Forms.TableLayoutPanel();
            this._statusValueLabel = new System.Windows.Forms.Label();
            this._statusValue = new System.Windows.Forms.Label();
            this._terminalValueLabel = new System.Windows.Forms.Label();
            this._terminalValue = new System.Windows.Forms.Label();
            this._workshopValueLabel = new System.Windows.Forms.Label();
            this._workshopValue = new System.Windows.Forms.Label();
            this._currentTaskValueLabel = new System.Windows.Forms.Label();
            this._currentTaskValue = new System.Windows.Forms.Label();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._pauseButton = new System.Windows.Forms.Button();
            this.testSoundButton = new System.Windows.Forms.Button();
            this.testSpeechButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this._taskList = new System.Windows.Forms.ListView();
            this.taskColumn5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.taskColumn6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.taskColumn7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.taskColumn8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.taskColumn9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.taskColumn10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._logTextBox = new System.Windows.Forms.TextBox();
            this.taskCaption = new System.Windows.Forms.Label();
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.trayMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.traySeparator = new System.Windows.Forms.ToolStripSeparator();
            this.trayMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this._notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.topPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.trayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.ColumnCount = 4;
            this.topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.topPanel.Controls.Add(this._statusValueLabel, 0, 0);
            this.topPanel.Controls.Add(this._statusValue, 1, 0);
            this.topPanel.Controls.Add(this._terminalValueLabel, 2, 0);
            this.topPanel.Controls.Add(this._terminalValue, 3, 0);
            this.topPanel.Controls.Add(this._workshopValueLabel, 0, 1);
            this.topPanel.Controls.Add(this._workshopValue, 1, 1);
            this.topPanel.Controls.Add(this._currentTaskValueLabel, 2, 1);
            this.topPanel.Controls.Add(this._currentTaskValue, 3, 1);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(16, 14, 16, 14);
            this.topPanel.RowCount = 2;
            this.topPanel.Size = new System.Drawing.Size(1336, 133);
            this.topPanel.TabIndex = 4;
            // 
            // _statusValueLabel
            // 
            this._statusValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._statusValueLabel.Location = new System.Drawing.Point(20, 14);
            this._statusValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._statusValueLabel.Name = "_statusValueLabel";
            this._statusValueLabel.Size = new System.Drawing.Size(132, 27);
            this._statusValueLabel.TabIndex = 0;
            this._statusValueLabel.Text = "运行状态：";
            this._statusValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _statusValue
            // 
            this._statusValue.AutoEllipsis = true;
            this._statusValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this._statusValue.Location = new System.Drawing.Point(160, 14);
            this._statusValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._statusValue.Name = "_statusValue";
            this._statusValue.Size = new System.Drawing.Size(401, 27);
            this._statusValue.TabIndex = 1;
            this._statusValue.Text = "未初始化";
            this._statusValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _terminalValueLabel
            // 
            this._terminalValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._terminalValueLabel.Location = new System.Drawing.Point(569, 14);
            this._terminalValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._terminalValueLabel.Name = "_terminalValueLabel";
            this._terminalValueLabel.Size = new System.Drawing.Size(132, 27);
            this._terminalValueLabel.TabIndex = 2;
            this._terminalValueLabel.Text = "终端编号：";
            this._terminalValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _terminalValue
            // 
            this._terminalValue.AutoEllipsis = true;
            this._terminalValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this._terminalValue.Location = new System.Drawing.Point(709, 14);
            this._terminalValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._terminalValue.Name = "_terminalValue";
            this._terminalValue.Size = new System.Drawing.Size(607, 27);
            this._terminalValue.TabIndex = 3;
            this._terminalValue.Text = "未初始化";
            this._terminalValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _workshopValueLabel
            // 
            this._workshopValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workshopValueLabel.Location = new System.Drawing.Point(20, 41);
            this._workshopValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._workshopValueLabel.Name = "_workshopValueLabel";
            this._workshopValueLabel.Size = new System.Drawing.Size(132, 78);
            this._workshopValueLabel.TabIndex = 4;
            this._workshopValueLabel.Text = "工厂 / 车间：";
            this._workshopValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _workshopValue
            // 
            this._workshopValue.AutoEllipsis = true;
            this._workshopValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this._workshopValue.Location = new System.Drawing.Point(160, 41);
            this._workshopValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._workshopValue.Name = "_workshopValue";
            this._workshopValue.Size = new System.Drawing.Size(401, 78);
            this._workshopValue.TabIndex = 5;
            this._workshopValue.Text = "未初始化";
            this._workshopValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _currentTaskValueLabel
            // 
            this._currentTaskValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._currentTaskValueLabel.Location = new System.Drawing.Point(569, 41);
            this._currentTaskValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._currentTaskValueLabel.Name = "_currentTaskValueLabel";
            this._currentTaskValueLabel.Size = new System.Drawing.Size(132, 78);
            this._currentTaskValueLabel.TabIndex = 6;
            this._currentTaskValueLabel.Text = "当前任务：";
            this._currentTaskValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _currentTaskValue
            // 
            this._currentTaskValue.AutoEllipsis = true;
            this._currentTaskValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this._currentTaskValue.Location = new System.Drawing.Point(709, 41);
            this._currentTaskValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._currentTaskValue.Name = "_currentTaskValue";
            this._currentTaskValue.Size = new System.Drawing.Size(607, 78);
            this._currentTaskValue.TabIndex = 7;
            this._currentTaskValue.Text = "未初始化";
            this._currentTaskValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this._pauseButton);
            this.buttonPanel.Controls.Add(this.testSoundButton);
            this.buttonPanel.Controls.Add(this.testSpeechButton);
            this.buttonPanel.Controls.Add(this.settingsButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonPanel.Location = new System.Drawing.Point(0, 133);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(16, 5, 16, 5);
            this.buttonPanel.Size = new System.Drawing.Size(1336, 51);
            this.buttonPanel.TabIndex = 3;
            // 
            // _pauseButton
            // 
            this._pauseButton.Location = new System.Drawing.Point(20, 8);
            this._pauseButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._pauseButton.Name = "_pauseButton";
            this._pauseButton.Size = new System.Drawing.Size(133, 27);
            this._pauseButton.TabIndex = 0;
            this._pauseButton.Text = "暂停播报";
            this._pauseButton.Click += new System.EventHandler(this.PauseButtonOnClick);
            // 
            // testSoundButton
            // 
            this.testSoundButton.Location = new System.Drawing.Point(161, 8);
            this.testSoundButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.testSoundButton.Name = "testSoundButton";
            this.testSoundButton.Size = new System.Drawing.Size(133, 27);
            this.testSoundButton.TabIndex = 1;
            this.testSoundButton.Text = "测试提示音";
            this.testSoundButton.Click += new System.EventHandler(this.TestSoundButtonOnClick);
            // 
            // testSpeechButton
            // 
            this.testSpeechButton.Location = new System.Drawing.Point(302, 8);
            this.testSpeechButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.testSpeechButton.Name = "testSpeechButton";
            this.testSpeechButton.Size = new System.Drawing.Size(153, 27);
            this.testSpeechButton.TabIndex = 2;
            this.testSpeechButton.Text = "测试中文语音";
            this.testSpeechButton.Click += new System.EventHandler(this.TestSpeechButtonOnClick);
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(463, 8);
            this.settingsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(133, 27);
            this.settingsButton.TabIndex = 3;
            this.settingsButton.Text = "系统设置";
            this.settingsButton.Click += new System.EventHandler(this.SettingsButtonOnClick);
            // 
            // _taskList
            // 
            this._taskList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.taskColumn5,
            this.taskColumn6,
            this.taskColumn7,
            this.taskColumn8,
            this.taskColumn9,
            this.taskColumn10});
            this._taskList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._taskList.FullRowSelect = true;
            this._taskList.GridLines = true;
            this._taskList.HideSelection = false;
            this._taskList.Location = new System.Drawing.Point(0, 214);
            this._taskList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._taskList.Name = "_taskList";
            this._taskList.Size = new System.Drawing.Size(1336, 344);
            this._taskList.TabIndex = 0;
            this._taskList.UseCompatibleStateImageBehavior = false;
            this._taskList.View = System.Windows.Forms.View.Details;
            // 
            // taskColumn5
            // 
            this.taskColumn5.Name = "taskColumn5";
            this.taskColumn5.Text = "任务 ID";
            this.taskColumn5.Width = 90;
            // 
            // taskColumn6
            // 
            this.taskColumn6.Name = "taskColumn6";
            this.taskColumn6.Text = "来源";
            this.taskColumn6.Width = 80;
            // 
            // taskColumn7
            // 
            this.taskColumn7.Name = "taskColumn7";
            this.taskColumn7.Text = "产线/工位";
            this.taskColumn7.Width = 180;
            // 
            // taskColumn8
            // 
            this.taskColumn8.Name = "taskColumn8";
            this.taskColumn8.Text = "播报内容";
            this.taskColumn8.Width = 400;
            // 
            // taskColumn9
            // 
            this.taskColumn9.Name = "taskColumn9";
            this.taskColumn9.Text = "次数";
            this.taskColumn9.Width = 80;
            // 
            // taskColumn10
            // 
            this.taskColumn10.Name = "taskColumn10";
            this.taskColumn10.Text = "状态";
            this.taskColumn10.Width = 90;
            // 
            // _logTextBox
            // 
            this._logTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._logTextBox.Font = new System.Drawing.Font("Courier New", 9F);
            this._logTextBox.Location = new System.Drawing.Point(0, 558);
            this._logTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._logTextBox.Multiline = true;
            this._logTextBox.Name = "_logTextBox";
            this._logTextBox.ReadOnly = true;
            this._logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._logTextBox.Size = new System.Drawing.Size(1336, 184);
            this._logTextBox.TabIndex = 2;
            // 
            // taskCaption
            // 
            this.taskCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.taskCaption.Location = new System.Drawing.Point(0, 184);
            this.taskCaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.taskCaption.Name = "taskCaption";
            this.taskCaption.Padding = new System.Windows.Forms.Padding(16, 7, 0, 0);
            this.taskCaption.Size = new System.Drawing.Size(1336, 30);
            this.taskCaption.TabIndex = 1;
            this.taskCaption.Text = "待播报任务（按创建时间 FIFO）";
            // 
            // trayMenu
            // 
            this.trayMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayMenuItem1,
            this.trayMenuItem2,
            this.trayMenuItem3,
            this.trayMenuItem4,
            this.trayMenuItem5,
            this.trayMenuItem6,
            this.traySeparator,
            this.trayMenuItem7});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(181, 178);
            // 
            // trayMenuItem1
            // 
            this.trayMenuItem1.Name = "trayMenuItem1";
            this.trayMenuItem1.Size = new System.Drawing.Size(180, 24);
            this.trayMenuItem1.Text = "打开主窗口";
            this.trayMenuItem1.Click += new System.EventHandler(this.RestoreWindowOnClick);
            // 
            // trayMenuItem2
            // 
            this.trayMenuItem2.Name = "trayMenuItem2";
            this.trayMenuItem2.Size = new System.Drawing.Size(180, 24);
            this.trayMenuItem2.Text = "开始/暂停播报";
            this.trayMenuItem2.Click += new System.EventHandler(this.PauseButtonOnClick);
            // 
            // trayMenuItem3
            // 
            this.trayMenuItem3.Name = "trayMenuItem3";
            this.trayMenuItem3.Size = new System.Drawing.Size(180, 24);
            this.trayMenuItem3.Text = "测试提示音";
            this.trayMenuItem3.Click += new System.EventHandler(this.TestSoundButtonOnClick);
            // 
            // trayMenuItem4
            // 
            this.trayMenuItem4.Name = "trayMenuItem4";
            this.trayMenuItem4.Size = new System.Drawing.Size(180, 24);
            this.trayMenuItem4.Text = "测试中文语音";
            this.trayMenuItem4.Click += new System.EventHandler(this.TestSpeechButtonOnClick);
            // 
            // trayMenuItem5
            // 
            this.trayMenuItem5.Name = "trayMenuItem5";
            this.trayMenuItem5.Size = new System.Drawing.Size(180, 24);
            this.trayMenuItem5.Text = "系统设置";
            this.trayMenuItem5.Click += new System.EventHandler(this.SettingsButtonOnClick);
            // 
            // trayMenuItem6
            // 
            this.trayMenuItem6.Name = "trayMenuItem6";
            this.trayMenuItem6.Size = new System.Drawing.Size(180, 24);
            this.trayMenuItem6.Text = "打开日志目录";
            this.trayMenuItem6.Click += new System.EventHandler(this.OpenLogDirectoryOnClick);
            // 
            // traySeparator
            // 
            this.traySeparator.Name = "traySeparator";
            this.traySeparator.Size = new System.Drawing.Size(177, 6);
            // 
            // trayMenuItem7
            // 
            this.trayMenuItem7.Name = "trayMenuItem7";
            this.trayMenuItem7.Size = new System.Drawing.Size(180, 24);
            this.trayMenuItem7.Text = "退出程序";
            this.trayMenuItem7.Click += new System.EventHandler(this.ExitProgramOnClick);
            // 
            // _notifyIcon
            // 
            this._notifyIcon.ContextMenuStrip = this.trayMenu;
            this._notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("_notifyIcon.Icon")));
            this._notifyIcon.Text = "MES 安灯语音播报";
            this._notifyIcon.DoubleClick += new System.EventHandler(this.RestoreWindowOnClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1336, 742);
            this.Controls.Add(this._taskList);
            this.Controls.Add(this.taskCaption);
            this.Controls.Add(this._logTextBox);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.topPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1167, 662);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MES 安灯语音播报";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormOnClosing);
            this.Load += new System.EventHandler(this.MainFormOnLoad);
            this.topPanel.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this.trayMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label _statusValueLabel;
        private Label _statusValue;
        private Label _terminalValueLabel;
        private Label _terminalValue;
        private Label _workshopValueLabel;
        private Label _workshopValue;
        private Label _currentTaskValueLabel;
        private Label _currentTaskValue;
        private TableLayoutPanel topPanel;
        private FlowLayoutPanel buttonPanel;
        private Button _pauseButton;
        private Button testSoundButton;
        private Button testSpeechButton;
        private Button settingsButton;
        private ListView _taskList;
        private TextBox _logTextBox;
        private Label taskCaption;
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip trayMenu;
        private ToolStripMenuItem trayMenuItem1;
        private ToolStripMenuItem trayMenuItem2;
        private ToolStripMenuItem trayMenuItem3;
        private ToolStripMenuItem trayMenuItem4;
        private ToolStripMenuItem trayMenuItem5;
        private ToolStripMenuItem trayMenuItem6;
        private ToolStripMenuItem trayMenuItem7;
        private ToolStripSeparator traySeparator;
        private ColumnHeader taskColumn5;
        private ColumnHeader taskColumn6;
        private ColumnHeader taskColumn7;
        private ColumnHeader taskColumn8;
        private ColumnHeader taskColumn9;
        private ColumnHeader taskColumn10;
    }
}
