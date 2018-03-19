﻿using System;
using System.IO;
using static TestNinja.Mocking.HouseKeeperService;

namespace TestNinja.Mocking
{

	public class StatementGenerator : IStatementGenerator
	{
		public string SaveStatement(int housekeeperOid, string housekeeperName, DateTime statementDate)
		{
			var report = new HousekeeperStatementReport(housekeeperOid, statementDate);

			if (!report.HasData)
				return string.Empty;

			report.CreateDocument();

			var filename = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				string.Format("Sandpiper Statement {0:yyyy-MM} {1}.pdf", statementDate, housekeeperName));

			report.ExportToPdf(filename);

			return filename;
		}
	}
}