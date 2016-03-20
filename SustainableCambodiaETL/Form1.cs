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
using System.Text.RegularExpressions;

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
                            c.ID,
                            c.LastName,
                            c.FirstName,
							(SELECT CONCAT(p.ID,p.Extension) 
								FROM sqlSCambodia.tblPhotos p
								INNER JOIN sqlSCambodia.tblChildPhotos cp on cp.PhotoID = p.ID
								WHERE cp.ChildID = c.ID and cp.PrimaryChildPhoto=1) AS PrimaryPhoto,";
            sql += "       '['+STUFF((SELECT CONCAT(',','{\"altPhoto\":\"',p.ID,p.Extension,'\"}')";
            sql += @"           FROM sqlSCambodia.tblPhotos p
								INNER JOIN sqlSCambodia.tblChildPhotos cp on cp.PhotoID = p.ID
								WHERE cp.ChildID = c.ID and cp.PrimaryChildPhoto=0 FOR XML PATH('')),1,1,'')+']' AS AdditionalPhotos,
                            c.Sponsored,
                            c.Gender,
                            c.DOB,c.Biography,
							c.ChildPhotoID,
                            c.DateCreated,
                            c.DateUpdated,
                            c.EligibleHomeSponsor,
                            c.EligibleSchoolSponsor,
                            c.EligibleScholarshipSponsor,
                            c.BiographyUpdated,
                            c.Deleted
                        FROM
                            sqlSCambodia.tblChildren c
                        WHERE
	                        Deleted = 0 
	                        AND (c.EligibleHomeSponsor = 1
	                        OR c.EligibleScholarshipSponsor = 1
	                        OR c.EligibleSchoolSponsor = 1)
						ORDER BY
							c.DateCreated DESC;
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
                child.biography = Regex.Replace(dr["Biography"].ToString(), "<.*?>", String.Empty);
                child.legacySponsored = Boolean.Parse(dr["Sponsored"].ToString());
                child.dob = DateTime.Parse(dr["DOB"].ToString());
                child.dateCreated = DateTime.Parse(dr["DateCreated"].ToString());
                child.dateUpdated = DateTime.Parse(dr["DateUpdated"].ToString());
                child.biographyUpdated = DateTime.Parse(dr["BiographyUpdated"].ToString());
                child.deleted = Boolean.Parse(dr["Deleted"].ToString());
                child.eligibleHomeSponsor = Boolean.Parse(dr["EligibleHomeSponsor"].ToString());
                child.eligibleScholarshipSponsor = Boolean.Parse(dr["EligibleScholarshipSponsor"].ToString());
                child.eligibleSchoolSponsor = Boolean.Parse(dr["EligibleSchoolSponsor"].ToString());
                child.primaryPhoto = dr["PrimaryPhoto"].ToString();
                child.additionalPhotos = dr["AdditionalPhotos"].ToString();
                children.Add(child);
            }
            JsonSerializer serializer = new JsonSerializer();
            if (saveFileDialog1.FileName != null)
            {
                Cursor = Cursors.WaitCursor;
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, children);
                }
                Cursor = Cursors.Default;
                MessageBox.Show("File exported.");

            }
            


        }
    }
}
