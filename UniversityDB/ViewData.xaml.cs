using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace UniversityDB
{    
    public partial class ViewData : Page
    {
        private readonly SqlConnection _con = new SqlConnection("Server=localhost;Database=ИС ВУЗа;Trusted_Connection=True;Encrypt=false");
        private readonly List<string> _queriesList;

        public ViewData()
        {
            InitializeComponent();            
            _queriesList = GetQueries();            
        }
        
        public void LoadGrid(string query)
        {
            using (var cmd = new SqlCommand(query, _con))
            {
                var dt = new DataTable();
                try
                {
                    _con.Open();
                    var sqlDataReader = cmd.ExecuteReader();
                    dt.Load(sqlDataReader);
                }
                catch (Exception ex)
                {
                    TextBlockInfo.Text = ex.Message;
                }
                finally
                {
                    _con.Close();
                }
                datagrid.ItemsSource = dt.DefaultView;
            }
        }

        private List<string> GetQueries()
        {
            using (FileStream fstream = File.OpenRead("Queries.json"))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string file = System.Text.Encoding.UTF8.GetString(array);
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
                return JsonConvert.DeserializeObject<List<string>>(file, settings);
            }
        }

        private void BTN_Execute(object sender, RoutedEventArgs e)
        {
            LoadGrid(TextBoxSQL.Text);            
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ComboBox1.SelectedItem as TextBlock;
            if (selectedItem != null)            
            {
                var selectedIndex = int.Parse(selectedItem.Text.Substring(8)) - 1;
                if (selectedIndex >= 0 && selectedIndex < _queriesList.Count)
                {
                    TextBoxSQL.Text = _queriesList[selectedIndex];
                }
            }
        }
    }
}
