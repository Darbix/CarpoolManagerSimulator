using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Carpool.Common.Tests
{
	public class XUnitTestOutputConverter : TextWriter
	{
		private readonly ITestOutputHelper _output;
		public XUnitTestOutputConverter(ITestOutputHelper output) => _output = output;
		public override Encoding Encoding => Encoding.UTF8;

		public override void WriteLine(string? message) => _output.WriteLine(message);

		public override void WriteLine(string format, params object?[] args) => _output.WriteLine(format, args);
	}
}
