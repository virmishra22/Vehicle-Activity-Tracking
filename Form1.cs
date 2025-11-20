using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Vehicle_Activity_Tracking
{
    public partial class Form1 : Form
    {
        classTransConnection oper = new classTransConnection();
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () => await GetAllVehiclesData());
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                Application.Exit();
            }
        }

        private async Task GetAllVehiclesData()
        {
            DateTime startTime = DateTime.Now;
            Logger.Write("===== Vehicle Activity Tracking Started =====");

            string token = await GetTokenAsync();
            if (token == null)
            {
                Logger.Write("ERROR: Token retrieval failed. Stopping process.");
                return;
            }

            List<string> vehicleList = await GetOwnVehiclesAsync();
            Logger.Write($"Total Vehicles Found: {vehicleList.Count}");

            int successCount = 0;
            int errorCount = 0;

            foreach (string reg in vehicleList)
            {
                try
                {
                    Logger.Write($"Processing Vehicle: {reg}");
                    var result = await GetVehicleDataAsync(reg, token);

                    if (result?.data != null)
                    {
                        InsertVehicleData(result.data, reg);
                        Logger.Write($"SUCCESS: Data inserted for {reg}");
                        successCount++;
                    }
                    else
                    {
                        Logger.Write($"WARNING: API returned no data for {reg}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write($"ERROR for {reg}: {ex.Message}");
                    errorCount++;
                }
            }         
            Logger.Write($"Successful: {successCount}");
            Logger.Write($"Errors: {errorCount}");  
            Logger.Write("===== Vehicle Activity Tracking Completed =====");
        }

        private async Task<List<string>> GetOwnVehiclesAsync()
        {
            List<string> vehicles = new List<string>();

            using (SqlConnection con = new SqlConnection(oper.abc("")))
            {
                string query = "SELECT vehicle_no FROM Vehicle_Master WHERE Vehicle_Type = 'OWN'";

                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        vehicles.Add(dr["vehicle_no"].ToString());
                    }
                }
            }

            return vehicles;
        }

        public async Task<string> GetTokenAsync()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("entryMode", "3");
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));                

                var json = JsonConvert.SerializeObject(new
                {
                    username = "Vinsum@123",
                    password = "Vinsum@123",
                    forapi = 1
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://xswift.biz/api/auth/token", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"HTTP {(int)response.StatusCode}\n\n{responseText}");
                    return null;
                }

                dynamic obj = JsonConvert.DeserializeObject(responseText);

                // READ TOKEN FROM data["access-token"]
                string token = obj?.data?["access-token"];

                return token;
            }
        }

        public async Task<VehicleDashboardResponse> GetVehicleDataAsync(string regNumber, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("entryMode", "3");
                client.DefaultRequestHeaders.Add("authkey", token);

                string url = $"https://xswift.biz/api/vehicle-dashboard/getVehicleData?regNumber={regNumber}";

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<VehicleDashboardResponse>(json);
            }
        }

        public void InsertVehicleData(VehicleData d, string regNumber)
        {
            using (SqlConnection con = new SqlConnection(oper.abc("")))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("usp_VehicleActivityTrackingDataInsert", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@RegNumber", regNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@totalDelayTime", (object)d.totalDelayTime ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@assignedTags", (object)d.assignedTags ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@address", (object)d.address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@mediumHaltSinceDuration", (object)d.mediumHaltSinceDuration ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@showStartTimeFirst", (object)d.showStartTimeFirst ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@showVehicle", (object)d.showVehicle ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@distLeft", (object)d.distLeft ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@remark", (object)d.remark ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@yesterdayKms", (object)d.yesterdayKms ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@dailyKms", (object)d.dailyKms ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@showTripStartName", (object)d.showTripStartName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@lrEwayExpiryTime", (object)d.lrEwayExpiryTime ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@etaStatus", (object)d.etaStatus ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@sourceDistanceInKm", (object)d.sourceDistanceInKm ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@eta", (object)d.eta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@etaInitial", (object)d.etaInitial ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@endName", (object)d.endName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@shareTripLink", (object)d.shareTripLink ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@driverMobile", (object)d.driverMobile ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@driverName", (object)d.driverName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@vehicleType", (object)d.vehicleType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@showPrimStatusV1", (object)d.showPrimStatusV1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@dttimePretty", (object)d.dttimePretty ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@startName", (object)d.startName ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
