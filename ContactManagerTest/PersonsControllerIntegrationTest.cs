
using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManagerTest
{
	public class PersonsControllerIntegrationTest:IClassFixture<CustomWebApplicationFactory>
	{
		private HttpClient _httpClient;
        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient=factory.CreateClient();
        }
		#region Index
		[Fact]
		public async Task Index_ToReturnView()
		{
			HttpResponseMessage response = await _httpClient.GetAsync("/Persons/Index");
			response.Should().BeSuccessful();
			string responsebody=await response.Content.ReadAsStringAsync();
		    HtmlDocument document = new HtmlDocument();
			document.LoadHtml(responsebody);
			var htmldocument = document.DocumentNode;
			htmldocument.QuerySelectorAll("table.persons").Should().NotBeNull();
		}
		#endregion
	}
}
