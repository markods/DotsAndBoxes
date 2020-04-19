namespace DotsAndBoxes
{
    partial class DotsAndBoxes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DotsAndBoxes));
            this.player1_cells_l = new System.Windows.Forms.Label();
            this.player1_score_cnt_l = new System.Windows.Forms.Label();
            this.player1_score_l = new System.Windows.Forms.Label();
            this.player1_name_t = new System.Windows.Forms.TextBox();
            this.player1_cell_cnt_l = new System.Windows.Forms.Label();
            this.player2_cells_l = new System.Windows.Forms.Label();
            this.player2_score_cnt_l = new System.Windows.Forms.Label();
            this.player2_score_l = new System.Windows.Forms.Label();
            this.player2_name_t = new System.Windows.Forms.TextBox();
            this.player2_cell_cnt_l = new System.Windows.Forms.Label();
            this.new_game_b = new System.Windows.Forms.Button();
            this.reset_score_b = new System.Windows.Forms.Button();
            this.undo_b = new System.Windows.Forms.Button();
            this.colcnt_t = new System.Windows.Forms.TextBox();
            this.rowcnt_t = new System.Windows.Forms.TextBox();
            this.board_size_l = new System.Windows.Forms.Label();
            this.controls_p = new System.Windows.Forms.Panel();
            this.player1_p = new System.Windows.Forms.Panel();
            this.player2_p = new System.Windows.Forms.Panel();
            this.board_p = new DoubleBufferedPanel();
            this.toolstrip_p = new System.Windows.Forms.Panel();
            this.menu_s = new System.Windows.Forms.MenuStrip();
            this.board_m = new System.Windows.Forms.ToolStripMenuItem();
            this.import_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.export_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.view_m = new System.Windows.Forms.ToolStripMenuItem();
            this.game_info_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.path_numbers_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.cell_coordinates_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.ai_moves_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.almost_circled_cells_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.controls_p.SuspendLayout();
            this.player1_p.SuspendLayout();
            this.player2_p.SuspendLayout();
            this.toolstrip_p.SuspendLayout();
            this.menu_s.SuspendLayout();
            this.SuspendLayout();
            // 
            // player1_cells_l
            // 
            this.player1_cells_l.BackColor = System.Drawing.Color.DarkGray;
            this.player1_cells_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.player1_cells_l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.player1_cells_l.Location = new System.Drawing.Point(7, 39);
            this.player1_cells_l.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.player1_cells_l.Name = "player1_cells_l";
            this.player1_cells_l.Size = new System.Drawing.Size(85, 23);
            this.player1_cells_l.TabIndex = 25;
            this.player1_cells_l.Text = "Cells :";
            this.player1_cells_l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // player1_score_cnt_l
            // 
            this.player1_score_cnt_l.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.player1_score_cnt_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.player1_score_cnt_l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.player1_score_cnt_l.Location = new System.Drawing.Point(84, 70);
            this.player1_score_cnt_l.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.player1_score_cnt_l.Name = "player1_score_cnt_l";
            this.player1_score_cnt_l.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.player1_score_cnt_l.Size = new System.Drawing.Size(99, 23);
            this.player1_score_cnt_l.TabIndex = 23;
            this.player1_score_cnt_l.Text = "0";
            this.player1_score_cnt_l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // player1_score_l
            // 
            this.player1_score_l.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.player1_score_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.player1_score_l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.player1_score_l.Location = new System.Drawing.Point(7, 70);
            this.player1_score_l.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.player1_score_l.Name = "player1_score_l";
            this.player1_score_l.Size = new System.Drawing.Size(85, 23);
            this.player1_score_l.TabIndex = 26;
            this.player1_score_l.Text = "Score:";
            this.player1_score_l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // player1_name_t
            // 
            this.player1_name_t.AllowDrop = true;
            this.player1_name_t.BackColor = System.Drawing.Color.LightGray;
            this.player1_name_t.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.player1_name_t.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.player1_name_t.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.player1_name_t.Location = new System.Drawing.Point(5, 5);
            this.player1_name_t.Margin = new System.Windows.Forms.Padding(4);
            this.player1_name_t.MaxLength = 16;
            this.player1_name_t.Name = "player1_name_t";
            this.player1_name_t.Size = new System.Drawing.Size(182, 28);
            this.player1_name_t.TabIndex = 5;
            this.player1_name_t.Text = "Player 1";
            this.player1_name_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.player1_name_t.WordWrap = false;
            // 
            // player1_cell_cnt_l
            // 
            this.player1_cell_cnt_l.BackColor = System.Drawing.Color.DarkGray;
            this.player1_cell_cnt_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.player1_cell_cnt_l.Location = new System.Drawing.Point(84, 39);
            this.player1_cell_cnt_l.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.player1_cell_cnt_l.Name = "player1_cell_cnt_l";
            this.player1_cell_cnt_l.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.player1_cell_cnt_l.Size = new System.Drawing.Size(99, 23);
            this.player1_cell_cnt_l.TabIndex = 11;
            this.player1_cell_cnt_l.Text = "0";
            this.player1_cell_cnt_l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // player2_cells_l
            // 
            this.player2_cells_l.BackColor = System.Drawing.Color.DarkGray;
            this.player2_cells_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.player2_cells_l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.player2_cells_l.Location = new System.Drawing.Point(8, 39);
            this.player2_cells_l.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.player2_cells_l.Name = "player2_cells_l";
            this.player2_cells_l.Size = new System.Drawing.Size(85, 23);
            this.player2_cells_l.TabIndex = 27;
            this.player2_cells_l.Text = "Cells :";
            this.player2_cells_l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // player2_score_cnt_l
            // 
            this.player2_score_cnt_l.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.player2_score_cnt_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.player2_score_cnt_l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.player2_score_cnt_l.Location = new System.Drawing.Point(84, 70);
            this.player2_score_cnt_l.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.player2_score_cnt_l.Name = "player2_score_cnt_l";
            this.player2_score_cnt_l.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.player2_score_cnt_l.Size = new System.Drawing.Size(99, 23);
            this.player2_score_cnt_l.TabIndex = 24;
            this.player2_score_cnt_l.Text = "0";
            this.player2_score_cnt_l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // player2_score_l
            // 
            this.player2_score_l.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.player2_score_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.player2_score_l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.player2_score_l.Location = new System.Drawing.Point(8, 70);
            this.player2_score_l.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.player2_score_l.Name = "player2_score_l";
            this.player2_score_l.Size = new System.Drawing.Size(85, 23);
            this.player2_score_l.TabIndex = 29;
            this.player2_score_l.Text = "Score:";
            this.player2_score_l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // player2_name_t
            // 
            this.player2_name_t.AcceptsTab = true;
            this.player2_name_t.BackColor = System.Drawing.Color.LightGray;
            this.player2_name_t.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.player2_name_t.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.player2_name_t.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.player2_name_t.Location = new System.Drawing.Point(5, 5);
            this.player2_name_t.Margin = new System.Windows.Forms.Padding(4);
            this.player2_name_t.MaxLength = 16;
            this.player2_name_t.Name = "player2_name_t";
            this.player2_name_t.Size = new System.Drawing.Size(182, 28);
            this.player2_name_t.TabIndex = 6;
            this.player2_name_t.Text = "AI3 8";
            this.player2_name_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.player2_name_t.WordWrap = false;
            // 
            // player2_cell_cnt_l
            // 
            this.player2_cell_cnt_l.BackColor = System.Drawing.Color.DarkGray;
            this.player2_cell_cnt_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.player2_cell_cnt_l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.player2_cell_cnt_l.Location = new System.Drawing.Point(84, 39);
            this.player2_cell_cnt_l.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.player2_cell_cnt_l.Name = "player2_cell_cnt_l";
            this.player2_cell_cnt_l.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.player2_cell_cnt_l.Size = new System.Drawing.Size(99, 23);
            this.player2_cell_cnt_l.TabIndex = 20;
            this.player2_cell_cnt_l.Text = "0";
            this.player2_cell_cnt_l.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // new_game_b
            // 
            this.new_game_b.BackColor = System.Drawing.Color.Gainsboro;
            this.new_game_b.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.new_game_b.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.new_game_b.Location = new System.Drawing.Point(9, 42);
            this.new_game_b.Margin = new System.Windows.Forms.Padding(4);
            this.new_game_b.Name = "new_game_b";
            this.new_game_b.Size = new System.Drawing.Size(151, 31);
            this.new_game_b.TabIndex = 0;
            this.new_game_b.Text = "New Game";
            this.new_game_b.UseVisualStyleBackColor = false;
            this.new_game_b.Click += new System.EventHandler(this.new_game_b_Click);
            // 
            // reset_score_b
            // 
            this.reset_score_b.BackColor = System.Drawing.Color.Gainsboro;
            this.reset_score_b.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.reset_score_b.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.reset_score_b.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.reset_score_b.Location = new System.Drawing.Point(9, 81);
            this.reset_score_b.Margin = new System.Windows.Forms.Padding(4);
            this.reset_score_b.Name = "reset_score_b";
            this.reset_score_b.Size = new System.Drawing.Size(151, 31);
            this.reset_score_b.TabIndex = 1;
            this.reset_score_b.Text = "Reset Score";
            this.reset_score_b.UseVisualStyleBackColor = false;
            this.reset_score_b.Click += new System.EventHandler(this.reset_score_b_Click);
            // 
            // undo_b
            // 
            this.undo_b.BackColor = System.Drawing.Color.Gainsboro;
            this.undo_b.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.undo_b.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.undo_b.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.undo_b.Location = new System.Drawing.Point(575, 10);
            this.undo_b.Margin = new System.Windows.Forms.Padding(4);
            this.undo_b.Name = "undo_b";
            this.undo_b.Size = new System.Drawing.Size(127, 102);
            this.undo_b.TabIndex = 2;
            this.undo_b.Text = "Undo";
            this.undo_b.UseVisualStyleBackColor = false;
            this.undo_b.Click += new System.EventHandler(this.undo_b_Click);
            // 
            // colcnt_t
            // 
            this.colcnt_t.AcceptsTab = true;
            this.colcnt_t.BackColor = System.Drawing.Color.LemonChiffon;
            this.colcnt_t.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.colcnt_t.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.colcnt_t.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.colcnt_t.Location = new System.Drawing.Point(127, 11);
            this.colcnt_t.Margin = new System.Windows.Forms.Padding(4);
            this.colcnt_t.MaxLength = 2;
            this.colcnt_t.Name = "colcnt_t";
            this.colcnt_t.Size = new System.Drawing.Size(27, 21);
            this.colcnt_t.TabIndex = 4;
            this.colcnt_t.Text = "5";
            this.colcnt_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colcnt_t.WordWrap = false;
            // 
            // rowcnt_t
            // 
            this.rowcnt_t.AcceptsTab = true;
            this.rowcnt_t.BackColor = System.Drawing.Color.LemonChiffon;
            this.rowcnt_t.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rowcnt_t.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.rowcnt_t.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rowcnt_t.Location = new System.Drawing.Point(87, 11);
            this.rowcnt_t.Margin = new System.Windows.Forms.Padding(4);
            this.rowcnt_t.MaxLength = 2;
            this.rowcnt_t.Name = "rowcnt_t";
            this.rowcnt_t.Size = new System.Drawing.Size(27, 21);
            this.rowcnt_t.TabIndex = 3;
            this.rowcnt_t.Text = "5";
            this.rowcnt_t.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.rowcnt_t.WordWrap = false;
            // 
            // board_size_l
            // 
            this.board_size_l.BackColor = System.Drawing.Color.LemonChiffon;
            this.board_size_l.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.board_size_l.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.board_size_l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.board_size_l.Location = new System.Drawing.Point(9, 10);
            this.board_size_l.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.board_size_l.Name = "board_size_l";
            this.board_size_l.Size = new System.Drawing.Size(150, 24);
            this.board_size_l.TabIndex = 13;
            this.board_size_l.Text = " Board:    x    ";
            this.board_size_l.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controls_p
            // 
            this.controls_p.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.controls_p.BackColor = System.Drawing.Color.Transparent;
            this.controls_p.Controls.Add(this.player1_p);
            this.controls_p.Controls.Add(this.reset_score_b);
            this.controls_p.Controls.Add(this.new_game_b);
            this.controls_p.Controls.Add(this.rowcnt_t);
            this.controls_p.Controls.Add(this.colcnt_t);
            this.controls_p.Controls.Add(this.board_size_l);
            this.controls_p.Controls.Add(this.undo_b);
            this.controls_p.Controls.Add(this.player2_p);
            this.controls_p.Location = new System.Drawing.Point(0, 0);
            this.controls_p.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.controls_p.MinimumSize = new System.Drawing.Size(711, 119);
            this.controls_p.Name = "controls_p";
            this.controls_p.Size = new System.Drawing.Size(711, 119);
            this.controls_p.TabIndex = 32;
            // 
            // player1_p
            // 
            this.player1_p.BackColor = System.Drawing.Color.DarkGray;
            this.player1_p.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.player1_p.Controls.Add(this.player1_score_cnt_l);
            this.player1_p.Controls.Add(this.player1_cell_cnt_l);
            this.player1_p.Controls.Add(this.player1_name_t);
            this.player1_p.Controls.Add(this.player1_cells_l);
            this.player1_p.Controls.Add(this.player1_score_l);
            this.player1_p.Location = new System.Drawing.Point(169, 10);
            this.player1_p.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.player1_p.Name = "player1_p";
            this.player1_p.Size = new System.Drawing.Size(195, 102);
            this.player1_p.TabIndex = 33;
            // 
            // player2_p
            // 
            this.player2_p.BackColor = System.Drawing.Color.DarkGray;
            this.player2_p.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.player2_p.Controls.Add(this.player2_cell_cnt_l);
            this.player2_p.Controls.Add(this.player2_score_cnt_l);
            this.player2_p.Controls.Add(this.player2_name_t);
            this.player2_p.Controls.Add(this.player2_score_l);
            this.player2_p.Controls.Add(this.player2_cells_l);
            this.player2_p.Location = new System.Drawing.Point(372, 10);
            this.player2_p.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.player2_p.Name = "player2_p";
            this.player2_p.Size = new System.Drawing.Size(195, 102);
            this.player2_p.TabIndex = 34;
            // 
            // board_p
            // 
            this.board_p.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.board_p.BackColor = System.Drawing.Color.Transparent;
            this.board_p.Location = new System.Drawing.Point(0, 27);
            this.board_p.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.board_p.MinimumSize = new System.Drawing.Size(711, 640);
            this.board_p.Name = "board_p";
            this.board_p.Size = new System.Drawing.Size(711, 640);
            this.board_p.TabIndex = 7;
            this.board_p.TabStop = true;
            this.board_p.MouseClick += new System.Windows.Forms.MouseEventHandler(this.board_p_MouseClick);
            // 
            // toolstrip_p
            // 
            this.toolstrip_p.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolstrip_p.BackColor = System.Drawing.Color.Silver;
            this.toolstrip_p.Controls.Add(this.controls_p);
            this.toolstrip_p.Location = new System.Drawing.Point(0, 667);
            this.toolstrip_p.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.toolstrip_p.MinimumSize = new System.Drawing.Size(711, 119);
            this.toolstrip_p.Name = "toolstrip_p";
            this.toolstrip_p.Size = new System.Drawing.Size(711, 119);
            this.toolstrip_p.TabIndex = 1;
            // 
            // menu_s
            // 
            this.menu_s.BackColor = System.Drawing.Color.Silver;
            this.menu_s.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menu_s.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.board_m,
            this.view_m});
            this.menu_s.Location = new System.Drawing.Point(0, 0);
            this.menu_s.Name = "menu_s";
            this.menu_s.Padding = new System.Windows.Forms.Padding(5, 1, 0, 1);
            this.menu_s.Size = new System.Drawing.Size(711, 26);
            this.menu_s.TabIndex = 8;
            this.menu_s.Text = "menuStrip1";
            // 
            // board_m
            // 
            this.board_m.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.import_mi,
            this.export_mi});
            this.board_m.Name = "board_m";
            this.board_m.Size = new System.Drawing.Size(61, 24);
            this.board_m.Text = "Board";
            // 
            // import_mi
            // 
            this.import_mi.Name = "import_mi";
            this.import_mi.Size = new System.Drawing.Size(138, 26);
            this.import_mi.Text = "Import...";
            this.import_mi.Click += new System.EventHandler(this.menu_file_import_Click);
            // 
            // export_mi
            // 
            this.export_mi.Name = "export_mi";
            this.export_mi.Size = new System.Drawing.Size(138, 26);
            this.export_mi.Text = "Export...";
            this.export_mi.Click += new System.EventHandler(this.menu_file_export_Click);
            // 
            // view_m
            // 
            this.view_m.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.game_info_mi,
            this.path_numbers_mi,
            this.cell_coordinates_mi,
            this.ai_moves_mi,
            this.almost_circled_cells_mi});
            this.view_m.Name = "view_m";
            this.view_m.Size = new System.Drawing.Size(53, 24);
            this.view_m.Text = "View";
            // 
            // game_info_mi
            // 
            this.game_info_mi.CheckOnClick = true;
            this.game_info_mi.Name = "game_info_mi";
            this.game_info_mi.Size = new System.Drawing.Size(216, 26);
            this.game_info_mi.Text = "Game Info";
            this.game_info_mi.CheckStateChanged += new System.EventHandler(this.game_info_mi_CheckStateChanged);
            // 
            // path_numbers_mi
            // 
            this.path_numbers_mi.CheckOnClick = true;
            this.path_numbers_mi.Name = "path_numbers_mi";
            this.path_numbers_mi.Size = new System.Drawing.Size(216, 26);
            this.path_numbers_mi.Text = "Path Numbers";
            this.path_numbers_mi.CheckStateChanged += new System.EventHandler(this.path_numbers_mi_CheckStateChanged);
            // 
            // cell_coordinates_mi
            // 
            this.cell_coordinates_mi.CheckOnClick = true;
            this.cell_coordinates_mi.Name = "cell_coordinates_mi";
            this.cell_coordinates_mi.Size = new System.Drawing.Size(216, 26);
            this.cell_coordinates_mi.Text = "Cell Coordinates";
            this.cell_coordinates_mi.CheckStateChanged += new System.EventHandler(this.cell_coordinates_mi_CheckStateChanged);
            // 
            // ai_moves_mi
            // 
            this.ai_moves_mi.CheckOnClick = true;
            this.ai_moves_mi.Name = "ai_moves_mi";
            this.ai_moves_mi.Size = new System.Drawing.Size(216, 26);
            this.ai_moves_mi.Text = "AI Moves";
            this.ai_moves_mi.CheckStateChanged += new System.EventHandler(this.ai_moves_mi_CheckStateChanged);
            // 
            // almost_circled_cells_mi
            // 
            this.almost_circled_cells_mi.CheckOnClick = true;
            this.almost_circled_cells_mi.Name = "almost_circled_cells_mi";
            this.almost_circled_cells_mi.Size = new System.Drawing.Size(216, 26);
            this.almost_circled_cells_mi.Text = "Almost Circled Cells";
            this.almost_circled_cells_mi.CheckStateChanged += new System.EventHandler(this.almost_circled_cells_mi_CheckStateChanged);
            // 
            // DotsAndBoxes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LemonChiffon;
            this.ClientSize = new System.Drawing.Size(711, 788);
            this.Controls.Add(this.toolstrip_p);
            this.Controls.Add(this.board_p);
            this.Controls.Add(this.menu_s);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(726, 825);
            this.Name = "DotsAndBoxes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dots and Boxes";
            this.controls_p.ResumeLayout(false);
            this.controls_p.PerformLayout();
            this.player1_p.ResumeLayout(false);
            this.player1_p.PerformLayout();
            this.player2_p.ResumeLayout(false);
            this.player2_p.PerformLayout();
            this.toolstrip_p.ResumeLayout(false);
            this.menu_s.ResumeLayout(false);
            this.menu_s.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button undo_b;
        private System.Windows.Forms.Button new_game_b;
        private System.Windows.Forms.Button reset_score_b;
        private System.Windows.Forms.TextBox rowcnt_t;
        private System.Windows.Forms.TextBox colcnt_t;
        private System.Windows.Forms.TextBox player1_name_t;
        private System.Windows.Forms.TextBox player2_name_t;
        private System.Windows.Forms.Label player1_cells_l;
        private System.Windows.Forms.Label player2_cells_l;
        private System.Windows.Forms.Label player1_cell_cnt_l;
        private System.Windows.Forms.Label player2_cell_cnt_l;
        private System.Windows.Forms.Label player1_score_l;
        private System.Windows.Forms.Label player2_score_l;
        private System.Windows.Forms.Label player1_score_cnt_l;
        private System.Windows.Forms.Label player2_score_cnt_l;
        private System.Windows.Forms.Label board_size_l;
        private System.Windows.Forms.Panel player1_p;
        private System.Windows.Forms.Panel player2_p;
        private System.Windows.Forms.Panel controls_p;
        private System.Windows.Forms.Panel toolstrip_p;
        private System.Windows.Forms.MenuStrip menu_s;
        private System.Windows.Forms.ToolStripMenuItem board_m;
        private System.Windows.Forms.ToolStripMenuItem import_mi;
        private System.Windows.Forms.ToolStripMenuItem export_mi;
        private DoubleBufferedPanel board_p;
        private System.Windows.Forms.ToolStripMenuItem view_m;
        private System.Windows.Forms.ToolStripMenuItem game_info_mi;
        private System.Windows.Forms.ToolStripMenuItem path_numbers_mi;
        private System.Windows.Forms.ToolStripMenuItem cell_coordinates_mi;
        private System.Windows.Forms.ToolStripMenuItem ai_moves_mi;
        private System.Windows.Forms.ToolStripMenuItem almost_circled_cells_mi;
    }
}

