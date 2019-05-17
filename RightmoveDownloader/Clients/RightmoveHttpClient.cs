using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace RightmoveDownloader.Clients
{
	public class RightmoveHttpClient : IRightmoveHttpClient
	{
		private readonly HttpClient httpClient;
		private readonly ILogger<RightmoveHttpClient> logger;

		public RightmoveHttpClient(HttpClient httpClient, ILogger<RightmoveHttpClient> logger)
		{
			this.httpClient = httpClient;
			this.logger = logger;
		}
		public async IAsyncEnumerable<IEnumerable<Property>> GetProperties(string locationIdentifier, int radius, int minBedrooms, int maxBedrooms, int minPrice, int maxPrice)
		{
			logger.LogInformation($"GetProperties({locationIdentifier}, {radius}, {minBedrooms}, {maxBedrooms}, {minPrice}, {maxPrice})");
			const int priceStep = 10;
			for (int i = minPrice; i <= maxPrice; i += priceStep)
			{
				string url = $"https://www.rightmove.co.uk/api/_search?locationIdentifier={locationIdentifier}&minBedrooms={minBedrooms}&maxBedrooms={maxBedrooms}&minPrice={i}&maxPrice={i + priceStep}&numberOfPropertiesPerPage=48&radius={radius}&sortType=6&includeLetAgreed=false&viewType=LIST&dontShow=retirement%2ChouseShare&channel=RENT&areaSizeUnit=sqm&currencyCode=GBP&isFetching=false&index=";
				var pageIndex = 0;
				while (true)
				{
					logger.LogInformation($"GetAsync({url + pageIndex})");
					var response = await httpClient.GetAsync(url + pageIndex);
					var result = await response.Content.ReadAsAsync<SearchResult>();
					yield return result.properties.Where(p => p.featuredProperty == false).Select(p =>
					{
						p.propertyUrl = new Uri(new Uri("https://www.rightmove.co.uk/"), p.propertyUrl).ToString();
						return p;
					});
					if (result.pagination.next == null) break;
					pageIndex = result.pagination.next.Value;
				}
			}
			logger.LogInformation($"GetProperties({locationIdentifier}, {radius}, {minBedrooms}, {maxBedrooms}, {minPrice}, {maxPrice}) - DONE");
		}

		class SearchResult
		{
			public Property[] properties { get; set; }
			public string resultCount { get; set; }
			public string searchParametersDescription { get; set; }
			public Radiusoption[] radiusOptions { get; set; }
			public Priceoption[] priceOptions { get; set; }
			public Bedroomoption[] bedroomOptions { get; set; }
			public Addedtositeoption[] addedToSiteOptions { get; set; }
			public Propertiesperpageoption[] propertiesPerPageOptions { get; set; }
			public Musthaveoption[] mustHaveOptions { get; set; }
			public Dontshowoption[] dontShowOptions { get; set; }
			public Furnishoption[] furnishOptions { get; set; }
			public Lettypeoption[] letTypeOptions { get; set; }
			public Sortoption[] sortOptions { get; set; }
			public Applicationproperties applicationProperties { get; set; }
			public string staticMapUrl { get; set; }
			public string shortLocationDescription { get; set; }
			public long timestamp { get; set; }
			public bool bot { get; set; }
			public string deviceType { get; set; }
			public Propertyschema propertySchema { get; set; }
			public Sidebarmodel sidebarModel { get; set; }
			public Seomodel seoModel { get; set; }
			public string mapViewUrl { get; set; }
			public string legacyUrl { get; set; }
			public string listViewUrl { get; set; }
			public string pageTitle { get; set; }
			public string metaDescription { get; set; }
			public Recentsearchmodel recentSearchModel { get; set; }
			public int maxCardsPerPage { get; set; }
			public string countryCode { get; set; }
			public int countryId { get; set; }
			public Currencycodeoption[] currencyCodeOptions { get; set; }
			public Areasizeunitoption[] areaSizeUnitOptions { get; set; }
			public Sizeoption[] sizeOptions { get; set; }
			public Pricetypeoption[] priceTypeOptions { get; set; }
			public bool showFeaturedAgent { get; set; }
			public bool commercialChannel { get; set; }
			public string disambiguationPagePath { get; set; }
			public Dfpmodel dfpModel { get; set; }
			public object noResultsModel { get; set; }
			public object urlPath { get; set; }
			public object tileGeometry { get; set; }
			public object[] geohashTerms { get; set; }
			public string comscore { get; set; }
			public Location1 location { get; set; }
			public Featureswitchstateforuser featureSwitchStateForUser { get; set; }
			public Pagination pagination { get; set; }
			public Searchparameters searchParameters { get; set; }
		}

		class Applicationproperties
		{
			public string mediaserverhost { get; set; }
			public string locationproductwebserverhost { get; set; }
			public string locationproductwebserverport { get; set; }
			public string publicsiteserverhost { get; set; }
			public string publicsiteserverport { get; set; }
			public string myrightmovewebserverhost { get; set; }
			public string myrightmovewebserverport { get; set; }
			public string analyticstypeformurl { get; set; }
			public bool clickstreamenabled { get; set; }
			public bool gaenabled { get; set; }
			public bool gtmenabled { get; set; }
			public string gtmid { get; set; }
			public string gtmauth { get; set; }
			public string gtmpreview { get; set; }
			public bool comscoreenabled { get; set; }
			public bool sentryenabled { get; set; }
			public string sentryappid { get; set; }
			public string sentryappkey { get; set; }
			public string infobuildversion { get; set; }
			public string metadataserviceurl { get; set; }
			public string sidebarmpuadUnitTimeout { get; set; }
			public string sidebarmpuadUnitPath { get; set; }
			public string customerLandingPageUrl { get; set; }
			public string dfpinterstitial1adUnitPath { get; set; }
			public string dfpinterstitial2adUnitPath { get; set; }
			public string dfpinterstitial3adUnitPath { get; set; }
			public bool optimizemappins { get; set; }
		}

		class Propertyschema
		{
			public int id { get; set; }
			public int bedrooms { get; set; }
			public int numberOfImages { get; set; }
			public int numberOfFloorplans { get; set; }
			public int numberOfVirtualTours { get; set; }
			public object summary { get; set; }
			public object displayAddress { get; set; }
			public object countryCode { get; set; }
			public Location location { get; set; }
			public object propertySubType { get; set; }
			public Listingupdate listingUpdate { get; set; }
			public bool premiumListing { get; set; }
			public bool featuredProperty { get; set; }
			public Price price { get; set; }
			public Customer customer { get; set; }
			public object distance { get; set; }
			public object transactionType { get; set; }
			public Productlabel productLabel { get; set; }
			public bool commercial { get; set; }
			public bool development { get; set; }
			public bool residential { get; set; }
			public bool students { get; set; }
			public bool auction { get; set; }
			public bool feesApply { get; set; }
			public string feesApplyText { get; set; }
			public string displaySize { get; set; }
			public bool showOnMap { get; set; }
			public string propertyUrl { get; set; }
			public string contactUrl { get; set; }
			public string channel { get; set; }
			public DateTime firstVisibleDate { get; set; }
			public object[] keywords { get; set; }
			public string keywordMatchType { get; set; }
			public string propertyTypeFullDescription { get; set; }
			public Propertyimages propertyImages { get; set; }
			public string displayStatus { get; set; }
			public string formattedBranchName { get; set; }
			public string addedOrReduced { get; set; }
			public bool isRecent { get; set; }
			public string formattedDistance { get; set; }
			public string heading { get; set; }
			public bool hasBrandPlus { get; set; }
		}

		class Location
		{
			public string latitude { get; set; }
			public string longitude { get; set; }
		}

		class Listingupdate
		{
			public object listingUpdateReason { get; set; }
			public DateTime listingUpdateDate { get; set; }
		}

		class Price
		{
			public int amount { get; set; }
			public string frequency { get; set; }
			public string currencyCode { get; set; }
			public Displayprice[] displayPrices { get; set; }
		}

		class Displayprice
		{
			public string displayPrice { get; set; }
			public string displayPriceQualifier { get; set; }
		}

		class Customer
		{
			public object branchId { get; set; }
			public object brandPlusLogoURI { get; set; }
			public object contactTelephone { get; set; }
			public object branchDisplayName { get; set; }
			public object branchName { get; set; }
			public object brandTradingName { get; set; }
			public object branchLandingPageUrl { get; set; }
			public bool development { get; set; }
			public bool showReducedProperties { get; set; }
			public bool commercial { get; set; }
			public bool showOnMap { get; set; }
			public string brandPlusLogoUrl { get; set; }
		}

		class Productlabel
		{
			public object productLabelText { get; set; }
		}

		class Propertyimages
		{
			public object[] images { get; set; }
			public string mainImageSrc { get; set; }
			public string mainImageSrcset { get; set; }
			public string mainMapImageSrc { get; set; }
			public string mainMapImageSrcset { get; set; }
		}

		class Sidebarmodel
		{
			public Soldhousepriceslinks soldHousePricesLinks { get; set; }
			public Relatedhousesearches relatedHouseSearches { get; set; }
			public Relatedflatsearches relatedFlatSearches { get; set; }
			public Relatedpopularsearches relatedPopularSearches { get; set; }
			public object relatedRegionsSearches { get; set; }
			public Channelswitchlink channelSwitchLink { get; set; }
			public object relatedStudentLinks { get; set; }
			public object mortgageMPU { get; set; }
			public object branchMPU { get; set; }
			public object countryGuideMPU { get; set; }
			public Suggestedlinks suggestedLinks { get; set; }
		}

		class Soldhousepriceslinks
		{
			public string heading { get; set; }
			public string subHeading { get; set; }
			public Model[] model { get; set; }
		}

		class Model
		{
			public string text { get; set; }
			public string url { get; set; }
			public bool noFollow { get; set; }
		}

		class Relatedhousesearches
		{
			public string heading { get; set; }
			public object subHeading { get; set; }
			public Model1[] model { get; set; }
		}

		class Model1
		{
			public string text { get; set; }
			public string url { get; set; }
			public bool noFollow { get; set; }
		}

		class Relatedflatsearches
		{
			public string heading { get; set; }
			public object subHeading { get; set; }
			public Model2[] model { get; set; }
		}

		class Model2
		{
			public string text { get; set; }
			public string url { get; set; }
			public bool noFollow { get; set; }
		}

		class Relatedpopularsearches
		{
			public string heading { get; set; }
			public object subHeading { get; set; }
			public Model3[] model { get; set; }
		}

		class Model3
		{
			public string text { get; set; }
			public string url { get; set; }
			public bool noFollow { get; set; }
		}

		class Channelswitchlink
		{
			public string heading { get; set; }
			public object subHeading { get; set; }
			public Model4[] model { get; set; }
		}

		class Model4
		{
			public string text { get; set; }
			public string url { get; set; }
			public bool noFollow { get; set; }
		}

		class Suggestedlinks
		{
			public string heading { get; set; }
			public object subHeading { get; set; }
			public Model5[] model { get; set; }
		}

		class Model5
		{
			public string text { get; set; }
			public string url { get; set; }
			public bool noFollow { get; set; }
		}

		class Seomodel
		{
			public string canonicalUrl { get; set; }
			public string metaRobots { get; set; }
		}

		class Recentsearchmodel
		{
			public string linkDisplayText { get; set; }
			public string titleDisplayText { get; set; }
			public string searchCriteriaMobile { get; set; }
			public long createDate { get; set; }
			public string locationIdentifierAndSearchType { get; set; }
		}

		class Dfpmodel
		{
			public object[] contentSlots { get; set; }
			public Sidebarslot[] sidebarSlots { get; set; }
			public Targeting[] targeting { get; set; }
		}

		class Sidebarslot
		{
			public string id { get; set; }
			public string adUnitPath { get; set; }
			public int[][] sizes { get; set; }
			public object[] mappings { get; set; }
		}

		class Targeting
		{
			public string key { get; set; }
			public string value { get; set; }
		}

		class Location1
		{
			public int id { get; set; }
			public string displayName { get; set; }
			public string shortDisplayName { get; set; }
			public string locationType { get; set; }
			public string listingCurrency { get; set; }
		}

		class Featureswitchstateforuser
		{
			public Individualfeatureswitchstate[] individualFeatureSwitchStates { get; set; }
			public Featureuser featureUser { get; set; }
		}

		class Featureuser
		{
			public string uniqueIdentifier { get; set; }
		}

		class Individualfeatureswitchstate
		{
			public string label { get; set; }
			public string state { get; set; }
			public bool shouldLog { get; set; }
		}

		class Pagination
		{
			public int total { get; set; }
			public Option[] options { get; set; }
			public string first { get; set; }
			public string last { get; set; }
			public int? next { get; set; }
			public int page { get; set; }
		}

		class Option
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Searchparameters
		{
			public string locationIdentifier { get; set; }
			public string numberOfPropertiesPerPage { get; set; }
			public string radius { get; set; }
			public string sortType { get; set; }
			public string index { get; set; }
			public object[] propertyTypes { get; set; }
			public bool includeLetAgreed { get; set; }
			public string viewType { get; set; }
			public object[] mustHave { get; set; }
			public object[] dontShow { get; set; }
			public object[] furnishTypes { get; set; }
			public string channel { get; set; }
			public string areaSizeUnit { get; set; }
			public string currencyCode { get; set; }
			public object[] keywords { get; set; }
		}
		[DebuggerDisplay("{id}")]
		public class Property
		{
			public string id { get; set; }
			public string nope { get; set; }
			public int bedrooms { get; set; }
			public int numberOfImages { get; set; }
			public int numberOfFloorplans { get; set; }
			public int numberOfVirtualTours { get; set; }
			public string summary { get; set; }
			public string displayAddress { get; set; }
			public string countryCode { get; set; }
			public Location2 location { get; set; }
			public string propertySubType { get; set; }
			public Listingupdate1 listingUpdate { get; set; }
			//public bool premiumListing { get; set; }
			public bool featuredProperty { get; set; }
			public Price1 price { get; set; }
			public Customer1 customer { get; set; }
			public object distance { get; set; }
			public string transactionType { get; set; }
			//public Productlabel1 productLabel { get; set; }
			public bool commercial { get; set; }
			public bool development { get; set; }
			public bool residential { get; set; }
			public bool students { get; set; }
			public bool auction { get; set; }
			public bool feesApply { get; set; }
			public string feesApplyText { get; set; }
			public string displaySize { get; set; }
			public bool showOnMap { get; set; }
			public string propertyUrl { get; set; }
			public string contactUrl { get; set; }
			public string channel { get; set; }
			public DateTime firstVisibleDate { get; set; }
			public object[] keywords { get; set; }
			public string keywordMatchType { get; set; }
			public string propertyTypeFullDescription { get; set; }
			public Propertyimages1 propertyImages { get; set; }
			public string displayStatus { get; set; }
			public string formattedBranchName { get; set; }
			//public string addedOrReduced { get; set; }
			//public bool isRecent { get; set; }
			public string formattedDistance { get; set; }
			public string heading { get; set; }
			public bool hasBrandPlus { get; set; }
			public DateTime? DeletedDate { get; set; }
		}

		public class Location2
		{
			public string latitude { get; set; }
			public string longitude { get; set; }
		}

		public class Listingupdate1
		{
			public string listingUpdateReason { get; set; }
			public DateTime listingUpdateDate { get; set; }
		}

		public class Price1
		{
			public int amount { get; set; }
			public string frequency { get; set; }
			public string currencyCode { get; set; }
			public Displayprice1[] displayPrices { get; set; }
		}

		public class Displayprice1
		{
			public string displayPrice { get; set; }
			public string displayPriceQualifier { get; set; }
		}

		public class Customer1
		{
			public int branchId { get; set; }
			public string brandPlusLogoURI { get; set; }
			public string contactTelephone { get; set; }
			public string branchDisplayName { get; set; }
			public string branchName { get; set; }
			public string brandTradingName { get; set; }
			public string branchLandingPageUrl { get; set; }
			public bool development { get; set; }
			public bool showReducedProperties { get; set; }
			public bool commercial { get; set; }
			public bool showOnMap { get; set; }
			public string brandPlusLogoUrl { get; set; }
		}

		class Productlabel1
		{
			public string productLabelText { get; set; }
		}

		public class Propertyimages1
		{
			public Image[] images { get; set; }
			public string mainImageSrc { get; set; }
			public string mainImageSrcset { get; set; }
			public string mainMapImageSrc { get; set; }
			public string mainMapImageSrcset { get; set; }
		}

		public class Image
		{
			public int order { get; set; }
			public string url { get; set; }
			public string mediaServerHost { get; set; }
			public string srcUrl { get; set; }
			public string srcsetUrl { get; set; }
			public string mapSrcUrl { get; set; }
			public string mapSrcsetUrl { get; set; }
		}

		class Radiusoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Priceoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Bedroomoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Addedtositeoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Propertiesperpageoption
		{
			public int value { get; set; }
			public string description { get; set; }
		}

		class Musthaveoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Dontshowoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Furnishoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Lettypeoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Sortoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Currencycodeoption
		{
			public string value { get; set; }
			public string description { get; set; }
		}

		class Areasizeunitoption
		{
			public string value { get; set; }
			public string description { get; set; }
			public string abbreviation { get; set; }
		}

		class Sizeoption
		{
			public string value { get; set; }
			public string description { get; set; }
			public string abbreviation { get; set; }
		}

		class Pricetypeoption
		{
			public string value { get; set; }
			public string description { get; set; }
			public string abbreviation { get; set; }
		}

	}
}