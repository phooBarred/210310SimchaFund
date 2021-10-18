using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _210310SimchaFund.Data
{
    public class ContributorsManager
    {
        private string _connectionString;

        public ContributorsManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Contributor> GetContributors()
        {
            List<Contributor> contributors = new();
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT *, 
                                (SELECT ISNULL(SUM(cs.Amount), 0) FROM Contributions cs WHERE cs.ContributorId = c.Id) + 
                                (SELECT ISNULL(SUM(d.Amount), 0) FROM Deposits d WHERE d.ContributorId = c.Id) AS Balance 
                                FROM Contributors c";
                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Contributor c = new Contributor
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        Cell = (string)reader["Cell"],
                        CreatedDate = (DateTime)reader["CreatedDate"],
                        AlwaysInclude = (bool)reader["AlwaysInclude"],
                        Balance = reader.GetOrNull<decimal>("Balance")
                    };
                    contributors.Add(c);
                }

                return contributors;
            }
        }

        public decimal GetTotalContributed()
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT 
                                    (
                                        SELECT SUM(Amount) 
                                        FROM Deposits
                                    )
                                    +
                                    (
                                        SELECT + SUM(Amount) 
                                        FROM Contributions
                                    )";

                conn.Open();
                return (decimal)cmd.ExecuteScalar();

            }
        }

        public string GetContributorName(int contributorId)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT FirstName + ' ' + LastName AS Name
                                    FROM Contributors
                                    WHERE Id = @contributorId";

                cmd.Parameters.AddWithValue("@contributorId", contributorId);
                conn.Open();
                return (string)cmd.ExecuteScalar();
            }
        }

        public List<Transaction> GetTransactions(int contributorId)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                List<Transaction> transactions = new();
                cmd.CommandText = @"SELECT 'Deposit' AS Action, d.Date, d.Amount 
                                    FROM Deposits d
                                    WHERE d.ContributorId = @contributorId

                                    UNION SELECT
                                    (
	                                    SELECT s.Name
	                                    FROM
	                                    Simchas s
	                                    WHERE s.Id = c.SimchaId
                                    ) 
	                                    AS Action, c.Date, c.Amount
                                    FROM Contributions c
                                    WHERE c.ContributorId = @contributorId
                                    ORDER BY Date Desc";

                cmd.Parameters.AddWithValue("@contributorId", contributorId);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Transaction transaction = new Transaction
                    {
                        Action = (string)reader["Action"],
                        Amount = (decimal)reader["Amount"],
                        Date = (DateTime)reader["Date"]
                    };
                    transactions.Add(transaction);
                }
                
                return transactions;
            }
        }

        public void NewContributor(Contributor contributor)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = $@"INSERT INTO Contributors (FirstName, LastName, Cell, CreatedDate, AlwaysInclude)
                                VALUES (@firstName, @lastName, @cell, @createdDate, @alwaysInclude) SELECT SCOPE_IDENTITY()";

                cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
                cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
                cmd.Parameters.AddWithValue("@cell", contributor.Cell);
                cmd.Parameters.AddWithValue("@createdDate", contributor.CreatedDate);
                cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                conn.Open();
                contributor.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void EditContributor(Contributor contributor)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = $@"UPDATE Contributors 
                                    SET FirstName = @firstName, LastName = @lastName, Cell = @cell, 
                                        CreatedDate = @createdDate, AlwaysInclude = @alwaysInclude
                                    WHERE Id = @id";

                cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
                cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
                cmd.Parameters.AddWithValue("@cell", contributor.Cell);
                cmd.Parameters.AddWithValue("@createdDate", contributor.CreatedDate);
                cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                cmd.Parameters.AddWithValue("@id", contributor.Id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void NewDeposit(Deposit deposit)
        {
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = $@"INSERT INTO Deposits (ContributorId, Amount, Date)
                                VALUES (@contributorId, @amount, @date)";

                cmd.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
                cmd.Parameters.AddWithValue("@amount", deposit.Amount);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString());
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
