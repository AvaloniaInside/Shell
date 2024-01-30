using Newtonsoft.Json;

namespace ShellBottomCustomNavigator.Models;

public partial class ProductDto
{
	[JsonProperty("ProductId")]
	public string ProductId { get; set; }

	[JsonProperty("Category")]
	public string Category { get; set; }

	[JsonProperty("MainCategory")]
	public string MainCategory { get; set; }

	[JsonProperty("SupplierName")]
	public string SupplierName { get; set; }

	[JsonProperty("Weight")]
	public string Weight { get; set; }

	[JsonProperty("WeightUnit")]
	public string WeightUnit { get; set; }

	[JsonProperty("ShortDescription")]
	public string ShortDescription { get; set; }

	[JsonProperty("Name")]
	public string Name { get; set; }

	[JsonProperty("PictureUrl")]
	public string PictureUrl { get; set; }

	[JsonProperty("Status")]
	public string Status { get; set; }

	[JsonProperty("Price")]
	// [JsonConverter(typeof(ParseStringConverter))]
	public decimal Price { get; set; }

	[JsonProperty("DimensionWidth")]
	// [JsonConverter(typeof(ParseStringConverter))]
	public double DimensionWidth { get; set; }

	[JsonProperty("DimensionDepth")]
	// [JsonConverter(typeof(ParseStringConverter))]
	public double DimensionDepth { get; set; }

	[JsonProperty("DimensionHeight")]
	public double DimensionHeight { get; set; }

	[JsonProperty("Unit")]
	public string Unit { get; set; }

	[JsonProperty("CurrencyCode")]
	public string CurrencyCode { get; set; }
}
