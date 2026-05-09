using System;
using System.ComponentModel;  // ← Нужно для IComponent
using System.Windows.Forms;

namespace ReservationApp
{
    internal class Form1Base : Form
    {
        private IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing); 
        }
    }
}