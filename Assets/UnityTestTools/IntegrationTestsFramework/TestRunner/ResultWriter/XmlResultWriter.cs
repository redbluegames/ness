// ****************************************************************
// Based on nUnit 2.6.2 (http://www.nunit.org/)
// ****************************************************************

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Text;

namespace UnityTest.IntegrationTestRunner
{

	/// <summary>
	/// Summary description for XmlResultWriter.
	/// </summary>
	public class XmlResultWriter
	{
		private XmlTextWriter xmlWriter;
		private string m_resultsName;

		#region Constructors

		public XmlResultWriter(string resultsName, string fileName)
		{
			xmlWriter = new XmlTextWriter(new StreamWriter(fileName,
															false,
															Encoding.UTF8));
			m_resultsName = resultsName;
		}

		#endregion

		private void InitializeXmlFile(ITestResult[] result)
		{
			var summaryResults = new ResultSummarizer(result);

			xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.WriteStartDocument(false);
			xmlWriter.WriteComment("This file represents the results of running a test suite");

			xmlWriter.WriteStartElement("test-results");

			xmlWriter.WriteAttributeString("name",
											m_resultsName);
			xmlWriter.WriteAttributeString("total",
											summaryResults.TestsRun.ToString());
			xmlWriter.WriteAttributeString("errors",
											summaryResults.Errors.ToString());
			xmlWriter.WriteAttributeString("failures",
											summaryResults.Failures.ToString());
			xmlWriter.WriteAttributeString("not-run",
											summaryResults.TestsNotRun.ToString());
			xmlWriter.WriteAttributeString("inconclusive",
											summaryResults.Inconclusive.ToString());
			xmlWriter.WriteAttributeString("ignored",
											summaryResults.Ignored.ToString());
			xmlWriter.WriteAttributeString("skipped",
											summaryResults.Skipped.ToString());
			xmlWriter.WriteAttributeString("invalid",
											summaryResults.NotRunnable.ToString());

			DateTime now = DateTime.Now;
			xmlWriter.WriteAttributeString("date",
											XmlConvert.ToString(now,
																"yyyy-MM-dd"));
			xmlWriter.WriteAttributeString("time",
											XmlConvert.ToString(now,
																"HH:mm:ss"));
			WriteEnvironment();
			WriteCultureInfo();
		}

		private void WriteCultureInfo()
		{
			xmlWriter.WriteStartElement("culture-info");
			xmlWriter.WriteAttributeString("current-culture",
											CultureInfo.CurrentCulture.ToString());
			xmlWriter.WriteAttributeString("current-uiculture",
											CultureInfo.CurrentUICulture.ToString());
			xmlWriter.WriteEndElement();
		}

		private void WriteEnvironment()
		{
			xmlWriter.WriteStartElement("environment");
			xmlWriter.WriteAttributeString("nunit-version",
											Assembly.GetExecutingAssembly().GetName().Version.ToString());
			xmlWriter.WriteAttributeString("clr-version",
											Environment.Version.ToString());
			xmlWriter.WriteAttributeString("os-version",
											Environment.OSVersion.ToString());
			xmlWriter.WriteAttributeString("platform",
											Environment.OSVersion.Platform.ToString());
			xmlWriter.WriteAttributeString("cwd",
											Environment.CurrentDirectory);
			xmlWriter.WriteAttributeString("machine-name",
											Environment.MachineName);
			xmlWriter.WriteAttributeString("user",
											Environment.UserName);
			xmlWriter.WriteAttributeString("user-domain",
											Environment.UserDomainName);
			xmlWriter.WriteEndElement();
		}

		#region Public Methods

		public void SaveTestResult(ITestResult[] results)
		{
			InitializeXmlFile(results);
			foreach (var result in results)
			{
				WriteResultElement(result);
			}
			TerminateXmlFile();
		}

		private void WriteResultElement(ITestResult result)
		{
			StartTestElement(result);

			switch (result.ResultState)
			{
				case TestResultState.Ignored:
				case TestResultState.NotRunnable:
				case TestResultState.Skipped:
					WriteReasonElement(result);
					break;

				case TestResultState.Failure:
				case TestResultState.Error:
				case TestResultState.Cancelled:
					WriteFailureElement(result);
					break;
				case TestResultState.Success:
				case TestResultState.Inconclusive:
					if (result.Message != null)
						WriteReasonElement(result);
					break;
			}

			xmlWriter.WriteEndElement(); // test element
		}

		private void TerminateXmlFile()
		{
				xmlWriter.WriteEndElement(); // test-results
				xmlWriter.WriteEndDocument();
				xmlWriter.Flush();

				xmlWriter.Close();
		}

		#endregion

		#region Element Creation Helpers

		private void StartTestElement(ITestResult result)
		{
			xmlWriter.WriteStartElement("test-case");
			xmlWriter.WriteAttributeString("name",
												result.FullName);

			xmlWriter.WriteAttributeString("executed",
											result.Executed.ToString());
			xmlWriter.WriteAttributeString("result",
											result.ResultState.ToString());

			if (result.Executed)
			{
				xmlWriter.WriteAttributeString("success",
												result.IsSuccess.ToString());
				xmlWriter.WriteAttributeString("time",
												result.Duration.ToString("#####0.000",
																	NumberFormatInfo.InvariantInfo));
			}
		}

		private void WriteReasonElement(ITestResult result)
		{
			xmlWriter.WriteStartElement("reason");
			xmlWriter.WriteStartElement("message");
			xmlWriter.WriteCData(result.Message);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

		private void WriteFailureElement(ITestResult result)
		{
			xmlWriter.WriteStartElement("failure");

			xmlWriter.WriteStartElement("message");
			WriteCData(result.Message);
			xmlWriter.WriteEndElement();

			xmlWriter.WriteStartElement("stack-trace");
			if (result.StackTrace != null)
				WriteCData(StackTraceFilter.Filter(result.StackTrace));
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		#endregion

		#region Output Helpers

		private void WriteCData(string text)
		{
			int start = 0;
			while (true)
			{
				int illegal = text.IndexOf("]]>",
											start);
				if (illegal < 0)
					break;
				xmlWriter.WriteCData(text.Substring(start,
													illegal - start + 2));
				start = illegal + 2;
				if (start >= text.Length)
					return;
			}

			if (start > 0)
				xmlWriter.WriteCData(text.Substring(start));
			else
				xmlWriter.WriteCData(text);
		}

		#endregion
	}
}
