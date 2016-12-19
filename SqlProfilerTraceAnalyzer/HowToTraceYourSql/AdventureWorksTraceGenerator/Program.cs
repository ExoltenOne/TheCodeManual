using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AdventureWorksTraceGenerator
{
    class Program
    {
        // AdventureWorks 2014 was used in this application
        // https://msftdbprodsamples.codeplex.com/downloads/get/880661
        const string ConnectionString = @"data source=.\SQLEXPRESS;initial catalog=AdventureWorks2014;integrated security=SSPI";

        static void Main()
        {
            GetStoreProcedures(10).ToList().Shuffle().ToList().ForEach(sp => ExecuteStoreProcedure(sp.Name, sp.Parameters));
        }

        private static void ExecuteStoreProcedure(string sprocName, IEnumerable<SqlParameterSet> parameters)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(sprocName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter.Name, parameter.Type).Value = parameter.Value;
                    }
                    
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static IEnumerable<StorePrecedureExecutionSet> GetStoreProcedures(int iterations)
        {
            for (var iteration = 0; iteration < iterations; iteration++)
            {
                for (var i = 0; i < 3; i++)
                {
                    yield return CreateGetBillOfMaterialsSprocExecution();
                }

                yield return CreateGetManagerEmployeesSprocExecution();

                for (var i = 0; i < 2; i++)
                {
                    yield return CreateUpdateEmployeeHireInfoSprocExecution();
                }
            }
        }

        private static StorePrecedureExecutionSet CreateUpdateEmployeeHireInfoSprocExecution()
        {
            return new StorePrecedureExecutionSet
            {
                Name = "[HumanResources].[uspUpdateEmployeeHireInfo]",
                Parameters = new List<SqlParameterSet>
                {
                    new SqlParameterSet
                    {
                        Name = "@BusinessEntityID",
                        Type = SqlDbType.Int,
                        Value = EmployeeHireInfoParametersProvider.GetBusinessEntityId()
                    },
                    new SqlParameterSet
                    {
                        Name = "@JobTitle",
                        Type = SqlDbType.NVarChar,
                        Value = EmployeeHireInfoParametersProvider.GetBusinessEntityId()
                    },
                    new SqlParameterSet
                    {
                        Name = "@HireDate",
                        Type = SqlDbType.DateTime,
                        Value = EmployeeHireInfoParametersProvider.GetHireDates()
                    },
                    new SqlParameterSet
                    {
                        Name = "@RateChangeDate",
                        Type = SqlDbType.DateTime,
                        Value = EmployeeHireInfoParametersProvider.GetRateChangeDate()
                    },
                    new SqlParameterSet
                    {
                        Name = "@Rate",
                        Type = SqlDbType.Money,
                        Value = EmployeeHireInfoParametersProvider.GetRate()
                    },
                    new SqlParameterSet
                    {
                        Name = "@PayFrequency",
                        Type = SqlDbType.TinyInt,
                        Value = EmployeeHireInfoParametersProvider.GetPayFrequency()
                    },
                    new SqlParameterSet
                    {
                        Name = "@CurrentFlag",
                        Type = SqlDbType.Bit,
                        Value = EmployeeHireInfoParametersProvider.GetCurrentFlag()
                    }
                }
            };
        }

        private static StorePrecedureExecutionSet CreateGetManagerEmployeesSprocExecution()
        {
            return new StorePrecedureExecutionSet
            {
                Name = "[dbo].[uspGetManagerEmployees]",
                Parameters = new List<SqlParameterSet>
                {
                    new SqlParameterSet
                    {
                        Name = "@BusinessEntityID",
                        Type = SqlDbType.Int,
                        Value = GetManagerEmployeesParameterProvider.GetBusinessEntityId()
                    }
                }
            };
        }

        private static StorePrecedureExecutionSet CreateGetBillOfMaterialsSprocExecution()
        {
            return new StorePrecedureExecutionSet
            {
                Name = "[dbo].[uspGetBillOfMaterials]",
                Parameters = new List<SqlParameterSet>
                {
                    new SqlParameterSet
                    {
                        Name = "@StartProductID",
                        Type = SqlDbType.Int,
                        Value = BillOfMaterialsSprocParametersProvider.GetStartProductId()
                    },
                    new SqlParameterSet
                    {
                        Name = "@CheckDate",
                        Type = SqlDbType.DateTime,
                        Value = BillOfMaterialsSprocParametersProvider.GetCheckDate()
                    }
                }
            };
        }
    }

    public static class BillOfMaterialsSprocParametersProvider
    {
        private static readonly Random Rng = new Random(Guid.NewGuid().GetHashCode());

        private static readonly IList<int> StartProductIds = new List<int>(100) {3,316,324,327,328,329,330,331,350,398,399,400,401,514,515,516,517,518,519,520,521,522,529,531,532,533,534,680,706,717,718,719,720,721,722,723,724,725,726,727,728,729,730,731,732,733,734,735,736,737,738,739,740,741,742,743,744,745,746,747,748,749,750,751,752,753,754,755,756,757,758,759,760,761,762,763,764,765,766,767,768,769,770,771,772,773,774,775,776,777,778,779,780,781,782,783,784,785,786,787};

        private static readonly IList<DateTime> CheckDates = new List<DateTime>(19) { new DateTime(2010,12,23), new DateTime(2010,08,19), new DateTime(2010,08,05), new DateTime(2010,03,18), new DateTime(2010,12,15), new DateTime(2010,06,09), new DateTime(2010,05,18), new DateTime(2010,10,05), new DateTime(2010,03,04), new DateTime(2010,09,15), new DateTime(2010,07,08), new DateTime(2010,05,06), new DateTime(2010,08,09), new DateTime(2010,05,26), new DateTime(2010,06,19), new DateTime(2010,05,04), new DateTime(2010,11,15), new DateTime(2010,07,26), new DateTime(2010,09,07) };

        public static int GetStartProductId()
        {
            return StartProductIds[Rng.Next(99)];
        }

        public static DateTime GetCheckDate()
        {
            return CheckDates[Rng.Next(19)];
        }
    }

    public static class GetManagerEmployeesParameterProvider
    {
        private static readonly Random Rng = new Random(Guid.NewGuid().GetHashCode());

        public static int GetBusinessEntityId()
        {
            return Rng.Next(1, 290);
        }
    }

    public static class EmployeeHireInfoParametersProvider
    {
        private static readonly Random Rng = new Random(Guid.NewGuid().GetHashCode());

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Rng.Next(s.Length)]).ToArray());
        }

        private static readonly IList<DateTime> Dates = new List<DateTime>(19) { new DateTime(2010, 12, 23), new DateTime(2010, 08, 19), new DateTime(2010, 08, 05), new DateTime(2010, 03, 18), new DateTime(2010, 12, 15), new DateTime(2010, 06, 09), new DateTime(2010, 05, 18), new DateTime(2010, 10, 05), new DateTime(2010, 03, 04), new DateTime(2010, 09, 15), new DateTime(2010, 07, 08), new DateTime(2010, 05, 06), new DateTime(2010, 08, 09), new DateTime(2010, 05, 26), new DateTime(2010, 06, 19), new DateTime(2010, 05, 04), new DateTime(2010, 11, 15), new DateTime(2010, 07, 26), new DateTime(2010, 09, 07) };

        public static int GetBusinessEntityId()
        {
            return Rng.Next(1, 290);
        }

        public static string GetJobTitle()
        {
            return RandomString(Rng.Next(25));
        }

        public static DateTime GetHireDates()
        {
            return Dates[Rng.Next(19)];
        }

        public static DateTime GetRateChangeDate()
        {
            return Dates[Rng.Next(19)];
        }

        public static decimal GetRate()
        {
            return Rng.Next(10,30) + (decimal)Rng.NextDouble();
        }

        public static int GetPayFrequency()
        {
            return Rng.Next(1, 2);
        }

        public static int GetCurrentFlag()
        {
            return Rng.Next(0, 1);
        }
    }

    public class StorePrecedureExecutionSet
    {
        public string Name { get; set; }

        public IList<SqlParameterSet> Parameters { get; set; }
    }

    public class SqlParameterSet
    {
        public string Name { get; set; }

        public SqlDbType Type { get; set; }

        public object Value { get; set; }
    }
}
