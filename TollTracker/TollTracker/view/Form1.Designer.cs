namespace TollTracker
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.reportProperties = new System.Windows.Forms.TabControl();
            this.vehicelTrackingPage = new System.Windows.Forms.TabPage();
            this.vehiclePicker = new System.Windows.Forms.ListBox();
            this.vehiceTollPage = new System.Windows.Forms.TabPage();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.monthlyRadioButton = new System.Windows.Forms.RadioButton();
            this.weeklyRadioButton = new System.Windows.Forms.RadioButton();
            this.dailyRadioButton = new System.Windows.Forms.RadioButton();
            this.vehiclePicker2 = new System.Windows.Forms.ListBox();
            this.tollSummaryPage = new System.Windows.Forms.TabPage();
            this.tollSummaryInfoLabel = new System.Windows.Forms.Label();
            this.gatereportPage = new System.Windows.Forms.TabPage();
            this.gatePicker = new System.Windows.Forms.ListBox();
            this.importButton = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.resultsGridView = new System.Windows.Forms.DataGridView();
            this.exportButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.reportProperties.SuspendLayout();
            this.vehicelTrackingPage.SuspendLayout();
            this.vehiceTollPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tollSummaryPage.SuspendLayout();
            this.gatereportPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultsGridView)).BeginInit();
            this.label1 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // reportProperties
            // 
            this.reportProperties.Controls.Add(this.vehicelTrackingPage);
            this.reportProperties.Controls.Add(this.vehiceTollPage);
            this.reportProperties.Controls.Add(this.tollSummaryPage);
            this.reportProperties.Controls.Add(this.gatereportPage);
            this.reportProperties.Location = new System.Drawing.Point(12, 12);
            this.reportProperties.Name = "reportProperties";
            this.reportProperties.SelectedIndex = 0;
            this.reportProperties.Size = new System.Drawing.Size(320, 301);
            this.reportProperties.TabIndex = 5;
            // 
            // vehicelTrackingPage
            // 
            this.vehicelTrackingPage.Controls.Add(this.vehiclePicker);
            this.vehicelTrackingPage.Location = new System.Drawing.Point(4, 22);
            this.vehicelTrackingPage.Name = "vehicelTrackingPage";
            this.vehicelTrackingPage.Padding = new System.Windows.Forms.Padding(3);
            this.vehicelTrackingPage.Size = new System.Drawing.Size(312, 275);
            this.vehicelTrackingPage.TabIndex = 0;
            this.vehicelTrackingPage.Text = "Vehicle Tracking";
            this.vehicelTrackingPage.UseVisualStyleBackColor = true;
            this.vehicelTrackingPage.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // vehiclePicker
            // 
            this.vehiclePicker.FormattingEnabled = true;
            this.vehiclePicker.Location = new System.Drawing.Point(3, 3);
            this.vehiclePicker.Name = "vehiclePicker";
            this.vehiclePicker.Size = new System.Drawing.Size(306, 95);
            this.vehiclePicker.TabIndex = 0;
            this.vehiclePicker.SelectedIndexChanged += new System.EventHandler(this.vehicleChooser_SelectedIndexChanged);
            // 
            // vehiceTollPage
            // 
            this.vehiceTollPage.Controls.Add(this.monthCalendar1);
            this.vehiceTollPage.Controls.Add(this.groupBox1);
            this.vehiceTollPage.Controls.Add(this.vehiclePicker2);
            this.vehiceTollPage.Location = new System.Drawing.Point(4, 22);
            this.vehiceTollPage.Name = "vehiceTollPage";
            this.vehiceTollPage.Padding = new System.Windows.Forms.Padding(3);
            this.vehiceTollPage.Size = new System.Drawing.Size(312, 275);
            this.vehiceTollPage.TabIndex = 1;
            this.vehiceTollPage.Text = "Vehicle Toll";
            this.vehiceTollPage.UseVisualStyleBackColor = true;
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(156, 107);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.monthlyRadioButton);
            this.groupBox1.Controls.Add(this.weeklyRadioButton);
            this.groupBox1.Controls.Add(this.dailyRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(3, 101);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(141, 168);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // monthlyRadioButton
            // 
            this.monthlyRadioButton.AutoSize = true;
            this.monthlyRadioButton.Location = new System.Drawing.Point(6, 65);
            this.monthlyRadioButton.Name = "monthlyRadioButton";
            this.monthlyRadioButton.Size = new System.Drawing.Size(97, 17);
            this.monthlyRadioButton.TabIndex = 7;
            this.monthlyRadioButton.TabStop = true;
            this.monthlyRadioButton.Text = "Monthly Report";
            this.monthlyRadioButton.UseVisualStyleBackColor = true;
            // 
            // weeklyRadioButton
            // 
            this.weeklyRadioButton.AutoSize = true;
            this.weeklyRadioButton.Location = new System.Drawing.Point(6, 42);
            this.weeklyRadioButton.Name = "weeklyRadioButton";
            this.weeklyRadioButton.Size = new System.Drawing.Size(96, 17);
            this.weeklyRadioButton.TabIndex = 6;
            this.weeklyRadioButton.Text = "Weekly Report";
            this.weeklyRadioButton.UseVisualStyleBackColor = true;
            // 
            // dailyRadioButton
            // 
            this.dailyRadioButton.AutoSize = true;
            this.dailyRadioButton.Checked = true;
            this.dailyRadioButton.Location = new System.Drawing.Point(6, 19);
            this.dailyRadioButton.Name = "dailyRadioButton";
            this.dailyRadioButton.Size = new System.Drawing.Size(83, 17);
            this.dailyRadioButton.TabIndex = 5;
            this.dailyRadioButton.TabStop = true;
            this.dailyRadioButton.Text = "Daily Report";
            this.dailyRadioButton.UseVisualStyleBackColor = true;
            // 
            // vehiclePicker2
            // 
            this.vehiclePicker2.FormattingEnabled = true;
            this.vehiclePicker2.Location = new System.Drawing.Point(0, 0);
            this.vehiclePicker2.Name = "vehiclePicker2";
            this.vehiclePicker2.Size = new System.Drawing.Size(306, 95);
            this.vehiclePicker2.TabIndex = 1;
            // 
            // tollSummaryPage
            // 
            this.tollSummaryPage.Controls.Add(this.tollSummaryInfoLabel);
            this.tollSummaryPage.Location = new System.Drawing.Point(4, 22);
            this.tollSummaryPage.Name = "tollSummaryPage";
            this.tollSummaryPage.Size = new System.Drawing.Size(312, 275);
            this.tollSummaryPage.TabIndex = 2;
            this.tollSummaryPage.Text = "Tolls Summary";
            this.tollSummaryPage.UseVisualStyleBackColor = true;
            // 
            // tollSummaryInfoLabel
            // 
            this.tollSummaryInfoLabel.AutoSize = true;
            this.tollSummaryInfoLabel.Location = new System.Drawing.Point(6, 10);
            this.tollSummaryInfoLabel.Name = "tollSummaryInfoLabel";
            this.tollSummaryInfoLabel.Size = new System.Drawing.Size(96, 13);
            this.tollSummaryInfoLabel.TabIndex = 0;
            this.tollSummaryInfoLabel.Text = "Just press Execute";
            // 
            // gatereportPage
            // 
            this.gatereportPage.Controls.Add(this.gatePicker);
            this.gatereportPage.Location = new System.Drawing.Point(4, 22);
            this.gatereportPage.Name = "gatereportPage";
            this.gatereportPage.Size = new System.Drawing.Size(312, 275);
            this.gatereportPage.TabIndex = 3;
            this.gatereportPage.Text = "Gate Report";
            this.gatereportPage.UseVisualStyleBackColor = true;
            // 
            // gatePicker
            // 
            this.gatePicker.FormattingEnabled = true;
            this.gatePicker.Location = new System.Drawing.Point(0, 0);
            this.gatePicker.Name = "gatePicker";
            this.gatePicker.Size = new System.Drawing.Size(309, 95);
            this.gatePicker.TabIndex = 0;
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(58, 319);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(75, 23);
            this.importButton.TabIndex = 6;
            this.importButton.Text = "Import Data";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(139, 319);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(75, 23);
            this.executeButton.TabIndex = 7;
            this.executeButton.Text = "Execute Report";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // resultsGridView
            // 
            this.resultsGridView.AllowUserToAddRows = false;
            this.resultsGridView.AllowUserToDeleteRows = false;
            this.resultsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultsGridView.Location = new System.Drawing.Point(341, 12);
            this.resultsGridView.Name = "resultsGridView";
            this.resultsGridView.ReadOnly = true;
            this.resultsGridView.Size = new System.Drawing.Size(313, 330);
            this.resultsGridView.TabIndex = 9;
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(220, 319);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 9;
            this.exportButton.Text = "Export Data";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(50, 85);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(644, 337);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 1200;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(807, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 350);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.resultsGridView);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.reportProperties);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.ClientSize = new System.Drawing.Size(1157, 480);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "TollTracker";
            this.reportProperties.ResumeLayout(false);
            this.vehicelTrackingPage.ResumeLayout(false);
            this.vehiceTollPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tollSummaryPage.ResumeLayout(false);
            this.tollSummaryPage.PerformLayout();
            this.gatereportPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resultsGridView)).EndInit();
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl reportProperties;
        private System.Windows.Forms.TabPage vehicelTrackingPage;
        private System.Windows.Forms.TabPage tollSummaryPage;
        private System.Windows.Forms.TabPage gatereportPage;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.DataGridView resultsGridView;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.TabPage vehiceTollPage;
        private System.Windows.Forms.ListBox vehiclePicker;
        private System.Windows.Forms.ListBox vehiclePicker2;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton monthlyRadioButton;
        private System.Windows.Forms.RadioButton weeklyRadioButton;
        private System.Windows.Forms.RadioButton dailyRadioButton;
        private System.Windows.Forms.Label tollSummaryInfoLabel;
        private System.Windows.Forms.ListBox gatePicker;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label2;
    }
}

