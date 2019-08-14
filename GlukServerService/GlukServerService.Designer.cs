using System;
using MySql.Data.MySqlClient;

namespace GlukServerService
{
    partial class GlukServerService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // GlukServerService
            // 
            this.ServiceName = "GlukServer";

        }

        #endregion
    }
}
