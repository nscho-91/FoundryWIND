using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezTools;

namespace ezAutoMom
{
    public partial class Model : Form
    {
        public string m_strModel;
        public bool m_bChange = false;

        int m_nModel = 0;
        Log m_log;
        ezGrid m_grid;
        ezRegistry m_reg = new ezRegistry("spAuto", "Model");

        Auto_Mom m_auto;

        public Model()
        {
            InitializeComponent();
            m_reg.Read("strModel", ref m_strModel);
        }

        public void Init(Auto_Mom auto)
        {
            m_auto = auto;
            m_log = new Log("Model", m_auto.ClassLogView());
            m_grid = new ezGrid("Model", grid, m_log, false);
            RunGrid(eGrid.eRegRead);
            RunGrid(eGrid.eInit); 
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            m_grid.PropertyChange(e);
            RunGrid(eGrid.eUpdate); RunGrid(eGrid.eRegWrite);
        }

        private void comboModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nModel;
            nModel = m_nModel;
            m_nModel = ((ComboBox)sender).SelectedIndex;
            m_strModel = (string)(((ComboBox)sender).SelectedItem);
            if (nModel != m_nModel) m_bChange = true;
            m_reg.Write("strModel", m_strModel);
        }

        public bool AddModel(string strModel)
        {
            comboModel.Items.Add(strModel);
            if (strModel != m_strModel) return false;
            m_nModel = (int)(comboModel.Items.Count - 1); 
            comboModel.SelectedIndex = m_nModel;
            return true;
        }

        public void RunGrid(eGrid eMode)
        {
            m_auto.ModelGrid(m_grid, eMode); 
        }

    }
}
