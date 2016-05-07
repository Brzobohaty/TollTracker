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
            this.vehicleTrackingPage = new System.Windows.Forms.TabPage();
            this.vehiclePicker = new System.Windows.Forms.ListBox();
            this.vehicleTollPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.monthlyRadioButton = new System.Windows.Forms.RadioButton();
            this.weeklyRadioButton = new System.Windows.Forms.RadioButton();
            this.dailyRadioButton = new System.Windows.Forms.RadioButton();
            this.vehiclePicker2 = new System.Windows.Forms.ListBox();
            this.tollsSummaryPage = new System.Windows.Forms.TabPage();
            this.tollSummaryInfoLabel = new System.Windows.Forms.Label();
            this.gateReportPage = new System.Windows.Forms.TabPage();
            this.gatePicker = new System.Windows.Forms.ListBox();
            this.importButton = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.reportProperties.SuspendLayout();
            this.vehicleTrackingPage.SuspendLayout();
            this.vehicleTollPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tollsSummaryPage.SuspendLayout();
            this.gateReportPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // reportProperties
            // 
            this.reportProperties.Controls.Add(this.vehicleTrackingPage);
            this.reportProperties.Controls.Add(this.vehicleTollPage);
            this.reportProperties.Controls.Add(this.tollsSummaryPage);
            this.reportProperties.Controls.Add(this.gateReportPage);
            this.reportProperties.Location = new System.Drawing.Point(12, 12);
            this.reportProperties.Name = "reportProperties";
            this.reportProperties.SelectedIndex = 0;
            this.reportProperties.Size = new System.Drawing.Size(320, 301);
            this.reportProperties.TabIndex = 5;
            // 
            // vehicleTrackingPage
            // 
            this.vehicleTrackingPage.Controls.Add(this.vehiclePicker);
            this.vehicleTrackingPage.Location = new System.Drawing.Point(4, 22);
            this.vehicleTrackingPage.Name = "vehicleTrackingPage";
            this.vehicleTrackingPage.Padding = new System.Windows.Forms.Padding(3);
            this.vehicleTrackingPage.Size = new System.Drawing.Size(312, 275);
            this.vehicleTrackingPage.TabIndex = 0;
            this.vehicleTrackingPage.Text = "Vehicle Tracking";
            this.vehicleTrackingPage.UseVisualStyleBackColor = true;
            // 
            // vehiclePicker
            // 
            this.vehiclePicker.FormattingEnabled = true;
            this.vehiclePicker.Location = new System.Drawing.Point(3, 3);
            this.vehiclePicker.Name = "vehiclePicker";
            this.vehiclePicker.Size = new System.Drawing.Size(306, 95);
            this.vehiclePicker.TabIndex = 0;
            // 
            // vehicleTollPage
            // 
            this.vehicleTollPage.Controls.Add(this.datePicker);
            this.vehicleTollPage.Controls.Add(this.groupBox1);
            this.vehicleTollPage.Controls.Add(this.vehiclePicker2);
            this.vehicleTollPage.Location = new System.Drawing.Point(4, 22);
            this.vehicleTollPage.Name = "vehicleTollPage";
            this.vehicleTollPage.Padding = new System.Windows.Forms.Padding(3);
            this.vehicleTollPage.Size = new System.Drawing.Size(312, 275);
            this.vehicleTollPage.TabIndex = 1;
            this.vehicleTollPage.Text = "Vehicle Toll";
            this.vehicleTollPage.UseVisualStyleBackColor = true;
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
            this.monthlyRadioButton.CheckedChanged += new System.EventHandler(this.monthlyRadioButton_CheckedChanged);
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
            this.weeklyRadioButton.CheckedChanged += new System.EventHandler(this.weeklyRadioButton_CheckedChanged);
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
            this.dailyRadioButton.CheckedChanged += new System.EventHandler(this.dailyRadioButton_CheckedChanged);
            // 
            // vehiclePicker2
            // 
            this.vehiclePicker2.FormattingEnabled = true;
            this.vehiclePicker2.Location = new System.Drawing.Point(0, 0);
            this.vehiclePicker2.Name = "vehiclePicker2";
            this.vehiclePicker2.Size = new System.Drawing.Size(306, 95);
            this.vehiclePicker2.TabIndex = 1;
            // 
            // tollsSummaryPage
            // 
            this.tollsSummaryPage.Controls.Add(this.tollSummaryInfoLabel);
            this.tollsSummaryPage.Location = new System.Drawing.Point(4, 22);
            this.tollsSummaryPage.Name = "tollsSummaryPage";
            this.tollsSummaryPage.Size = new System.Drawing.Size(312, 275);
            this.tollsSummaryPage.TabIndex = 2;
            this.tollsSummaryPage.Text = "Tolls Summary";
            this.tollsSummaryPage.UseVisualStyleBackColor = true;
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
            // gateReportPage
            // 
            this.gateReportPage.Controls.Add(this.gatePicker);
            this.gateReportPage.Location = new System.Drawing.Point(4, 22);
            this.gateReportPage.Name = "gateReportPage";
            this.gateReportPage.Size = new System.Drawing.Size(312, 275);
            this.gateReportPage.TabIndex = 3;
            this.gateReportPage.Text = "Gate Report";
            this.gateReportPage.UseVisualStyleBackColor = true;
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
            this.label1.Location = new System.Drawing.Point(9, 375);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(338, 28);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(408, 337);
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
            this.label2.Location = new System.Drawing.Point(338, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 2;
            // 
            // datePicker
            // 
            this.datePicker.CustomFormat = "dd. MM. yyyy";
            this.datePicker.Location = new System.Drawing.Point(150, 116);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(135, 20);
            this.datePicker.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 393);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.reportProperties);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "TollTracker";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.reportProperties.ResumeLayout(false);
            this.vehicleTrackingPage.ResumeLayout(false);
            this.vehicleTollPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tollsSummaryPage.ResumeLayout(false);
            this.tollsSummaryPage.PerformLayout();
            this.gateReportPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl reportProperties;
        private System.Windows.Forms.TabPage vehicleTrackingPage;
        private System.Windows.Forms.TabPage tollsSummaryPage;
        private System.Windows.Forms.TabPage gateReportPage;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.TabPage vehicleTollPage;
        private System.Windows.Forms.ListBox vehiclePicker;
        private System.Windows.Forms.ListBox vehiclePicker2;
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
        private System.Windows.Forms.DateTimePicker datePicker;
    }
}

