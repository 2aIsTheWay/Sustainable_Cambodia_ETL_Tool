using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SustainableCambodiaETL.lib;
using Newtonsoft.Json;
using System.IO;

namespace SustainableCambodiaETL
{
    public partial class Form1 : Form
    {
        DataSet ds = new DataSet();
        public Form1()
        {
            InitializeComponent();
            initializeDatabase();           
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            tbSavePathAndFile.Text = saveFileDialog1.FileName;
        }

        private void initializeDatabase()
        {
            string connectionString = "Server= localhost; Database= SCambodia;Integrated Security = SSPI;";
            string sql = @"SELECT
                            ID,
                            LastName,
                            FirstName,
                            Sponsored,
                            Gender,
                            DOB,Biography,
                            DateCreated,
                            DateUpdated,
                            EligibleHomeSponsor,
                            EligibleSchoolSponsor,
                            EligibleScholarshipSponsor,
                            BiographyUpdated,
                            Deleted
                        FROM
                            sqlSCambodia.tblChildren
                        WHERE
	                        Deleted = 0 
	                        AND (EligibleHomeSponsor = 1
	                        OR EligibleHomeSponsor = 1
	                        OR EligibleSchoolSponsor = 1);
            ";
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = ds.Tables[0];

            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed!" + ex.ToString());
            }
        }

        private void btnBuildJSON_Click(object sender, EventArgs e)
        {
            Child child; 
            List<Child> children = new List<Child>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                child = new Child();
                child.firstName = dr["FirstName"].ToString();
                child.lastName = dr["LastName"].ToString();
                child.gender = dr["Gender"].ToString();
                child.biography = dr["Biography"].ToString();
                child.legacySponsored = Boolean.Parse(dr["Sponsored"].ToString());
                child.dob = DateTime.Parse(dr["DOB"].ToString());
                child.dateCreated = DateTime.Parse(dr["DateCreated"].ToString());
                child.dateUpdated = DateTime.Parse(dr["DateUpdated"].ToString());
                child.biographyUpdated = DateTime.Parse(dr["BiographyUpdated"].ToString());
                child.deleted = Boolean.Parse(dr["Deleted"].ToString());
                child.eligibleHomeSponsor = Boolean.Parse(dr["EligibleHomeSponsor"].ToString());
                child.eligibleScholarshipSponsor = Boolean.Parse(dr["EligibleScholarshipSponsor"].ToString());
                child.eligibleSchoolSponsor = Boolean.Parse(dr["EligibleSchoolSponsor"].ToString());
                children.Add(child);
            }
            JsonSerializer serializer = new JsonSerializer();
            if (saveFileDialog1.FileName != null)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, children);
                }

            }
            


        }
    }
}
