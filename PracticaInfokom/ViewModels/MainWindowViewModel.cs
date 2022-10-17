using PersonnelAccountingSystem.ViewModels.Base;
using PracticaInfokom.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PracticaInfokom.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private DataTable crats;
        private DataTable slyabs;

        public DataTable Slyabs
        {
            get => slyabs;
            set
            {
                slyabs = value;
                OnPropertyChange();
            }
        }

        public DataTable Crats
        {
            get => crats;
            set
            {
                crats = value;
                OnPropertyChange();
            }
        }

        private string datebegin = DateTime.Now.ToString();

        public string DateBegin
        {
            get => datebegin;
            set
            {
                datebegin = value;
            }
        }

        private string dateend = DateTime.Now.ToString();

        public string DateEnd
        {
            get => dateend;
            set
            {
                dateend = value;
            }
        }

        public ICommand UpdateButton { get; }

        private bool UpdateButtonCanExecute(object o) => true;
        private void UpdateBUttonOnExecuted(object o)
        {
            using (var connection = new SqlConnection("Data Source=DESKTOP-STGMEUB;Initial Catalog=DBPractica;Integrated Security=True"))
            {
                DateTime upd_begin;
                DateTime.TryParse(DateBegin, out upd_begin);
                DateTime upd_end;
                DateTime.TryParse(DateEnd, out upd_end);
                connection.Open();
                SqlCommand com = new SqlCommand($"SELECT \"Дата-время начала (план)\", \"№ задания\", \"№ ПЗ\", \"Толщина (план)\", \"Ширина (план)\", \"Вес (план)\", \"Длина (ПЗ)\", \"Длина (по разметке)\" FROM Practica WHERE \"Дата-время начала (план)\" >= '{upd_begin}' AND \"Дата-время начала (план)\" <='{upd_end}'", connection);
                SqlDataReader reader = com.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                Slyabs = dt;

                com = new SqlCommand($"select \"№ ПЗ\", \"Длина (по разметке)\", \"Ширина (план)\", \"Вес (план)\", Round(\"Длина (Пз)\"/TRY_CAST(\"Длина (по разметке)\" AS FLOAT),0) AS \"Количество\" FROM Practica as P WHERE \"Дата-время начала (план)\" >= '{upd_begin}' AND \"Дата-время начала (план)\" <='{upd_end}'", connection);
                reader = com.ExecuteReader();
                dt = new DataTable();
                dt.Load(reader);
                Crats = dt;

                connection.Close();
                connection.Dispose();
            }
        }

        public MainWindowViewModel()
        {
            UpdateButton = new LyambdaCommand(UpdateBUttonOnExecuted, UpdateButtonCanExecute);
        }
    }
}
