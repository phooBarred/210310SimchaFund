using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace _210310SimchaFund.Data
{
    public class SimchasManager
    {
        private readonly string _connectionString;

        public SimchasManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Simcha> GetSimchas()
        {
            List<Simcha> simchas = new();
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT *,
                                (SELECT COUNT(*) FROM Contributions WHERE SimchaId = s.Id) as Contributors,
                                (SELECT ISNULL(SUM(Amount), 0)*-1 FROM Contributions WHERE SimchaId = s.Id) as TotalContributed
                                FROM Simchas s";
                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Simcha s = new()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Date = (DateTime)reader["Date"],
                        Contributors = (int)reader["Contributors"],
                        TotalContributed = reader.GetOrNull<decimal>("TotalContributed")
                    };
                    simchas.Add(s);
                }

                return simchas;
            }
        }

        public List<Contributor> GetContributors()
        {
            List<Contributor> contributors = new();
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //cmd.CommandText = @"SELECT * FROM Contributors";
                cmd.CommandText = @"SELECT *,
                                    (SELECT ISNULL(SUM(cs.Amount), 0) FROM Contributions cs WHERE cs.ContributorId = c.Id)
                                    +
                                    (SELECT ISNULL(SUM(d.Amount), 0) FROM Deposits d WHERE d.ContributorId = c.Id) AS Balance
                                    FROM Contributors c";
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Contributor c = new();
                    c.Id = (int)reader["Id"];
                    c.FirstName = (string)reader["FirstName"];
                    c.LastName = (string)reader["LastName"];
                    c.Cell = (string)reader["Cell"];
                    c.CreatedDate = (DateTime)reader["CreatedDate"];
                    c.AlwaysInclude = (bool)reader["AlwaysInclude"];
                    c.Balance = (decimal)reader["Balance"];
                    contributors.Add(c);
                }

                return contributors;
            }
        }

        public int GetContributorsCount()
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public decimal GetContributedToSimchaAmount(int simchaId)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT ISNULL(SUM(Amount), 0) FROM Contributions WHERE ID = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simchaId);
                conn.Open();
                return (decimal)cmd.ExecuteScalar();
            }
        }

        public List<SimchaContributor> GetSimchaContributors(int simchaId)
        {
            var contributors = GetContributors();

            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Contributions WHERE SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simchaId);
                conn.Open();
                var reader = cmd.ExecuteReader();
                List<Contribution> contributions = new();
                while (reader.Read())
                {
                    Contribution c = new();
                    c.ContributorId = (int)reader["ContributorId"];
                    c.Amount = (decimal)reader["Amount"] * -1;
                    c.Date = (DateTime)reader["Date"];
                    contributions.Add(c);
                }
                return contributors.ConvertAll(contributor =>
                {
                    var sc = new SimchaContributor
                    {
                        ContributorId = contributor.Id,
                        FirstName = contributor.FirstName,
                        LastName = contributor.LastName,
                        AlwaysInclude = contributor.AlwaysInclude,
                        Balance = contributor.Balance
                    };

                     Contribution contribution = contributions.FirstOrDefault(c => c.ContributorId == sc.ContributorId);
                    if (contribution != null)
                    {
                        sc.Amount = contribution.Amount;
                    }

                    return sc;
                });
            }
        }

        public Simcha GetSimcha(int id)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM Simchas
                                    WHERE Id = @id";

                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    return null;
                }

                return new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    Contributors = GetContributorsCount(),
                    TotalContributed = GetContributedToSimchaAmount(id)
                };
            }
        }

        public List<Contribution> GetContributionsByContributor(int id)
        {
            List<Contribution> contributions = new();

            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT c.*, s.Name FROM Contributions c
                                LEFT JOIN Simchas s
                                ON c.SimchaId = s.Id
                                WHERE c.ContributorId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                conn.Open();
                while (reader.Read())
                {
                    Contribution c = new();
                    c.ContributorId = (int)reader["ContributorId"];
                    c.SimchaId = (int)reader["SimchaId"];
                    c.SimchaName = (string)reader["Name"];
                    c.Amount = (decimal)reader["Amount"];
                    c.Date = (DateTime)reader["Date"];
                    contributions.Add(c);
                }
            }
            return contributions;
        }

        public List<Deposit> GetDepositsByContributor(int id)
        {
            List<Deposit> deposits = new();

            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Deposits WHERE Id = @id";

                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                conn.Open();
                while (reader.Read())
                {
                    Deposit d = new();
                    d.Id = (int)reader["Id"];
                    d.ContributorId = (int)reader["ContributorId"];
                    d.Amount = (decimal)reader["Amount"];
                    d.Date = (DateTime)reader["Date"];
                    deposits.Add(d);
                }
                return deposits;
            }
        }

        public void AddSimcha(Simcha simcha)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Simchas (Name, Date) VALUES(@name, @date) SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@name", simcha.Name);
                cmd.Parameters.AddWithValue("@date", simcha.Date);
                conn.Open();
                simcha.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void AddContributor(Contributor contributor)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Contributors (FirstName, LastName, Cell, CreatedDate, AlwayInclude)
                                @firstName, @lastName, @cell, @createdDate, @alwaysInclude";
                cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
                cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
                cmd.Parameters.AddWithValue("@cell", contributor.Cell);
                cmd.Parameters.AddWithValue("@date", contributor.CreatedDate);
                cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddContribution(Contribution contribution)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Simchas (ContributorId, SimchaId, Date, Amount)
                                @contributorId, @simchaId, @date, @amount";
                cmd.Parameters.AddWithValue("@contributorId", contribution.ContributorId);
                cmd.Parameters.AddWithValue("@simchaId", contribution.SimchaId);
                cmd.Parameters.AddWithValue("@date", contribution.Date);
                cmd.Parameters.AddWithValue("@amount", -System.Math.Abs(contribution.Amount));
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateContributions(List<ContributionToSimcha> contributionToSimchas)
        {
            using (SqlConnection connection = new(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DELETE from Contributions where SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", contributionToSimchas[0].SimchaId);

                connection.Open();
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.CommandText = @"INSERT INTO Contributions (ContributorId, SimchaId, Date, Amount)
                                    VALUES (@contributorId, @simchaId, @date, @amount)";
                foreach (var cts in contributionToSimchas)
                {
                    decimal.TryParse(cts.Amount, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out decimal amount);
                    if (cts.Include && amount > 0)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@contributorId", cts.ContributorId);
                        cmd.Parameters.AddWithValue("@simchaId", cts.SimchaId);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@amount", amount * -1);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void AddDeposit(Deposit deposit)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Simchas (ContributorId, Amount, Date)
                                @contributorId, @amount, @date";
                cmd.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
                cmd.Parameters.AddWithValue("@amount", deposit.Amount);
                cmd.Parameters.AddWithValue("@date", deposit.Date);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}