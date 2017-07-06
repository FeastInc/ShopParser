using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParser.HelperClasses;

namespace DataParser.Examples
{
    class TdDvorikiExample
    {
        public static void Parse()
        {
            var parser = new LiquiMolyClass(
                isCategory: node => node
                    ._SelectNodes(@".//*[@id='appPartZoomCompprd1appPart1zvc_Zoom_ProductBundle__0_0_def_0']")
                    .Count == 0,
                findProducts: (node, args) =>
                {
                    var a = node
                        ._SelectNodes(@"//a[contains(@href, 'product')]");
                    return node
                        ._SelectNodes(@"//*[contains(@class,'pgg_cg1_galleryDisplayer')]/div/a")
                        .Select(x => new ArgumentObject(x.Attributes["href"].Value))
                        .ToArray();
                },
                singlePropertiesCategory: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => "ТД-Дворики",
                },
                singlePropertiesProduct: new Dictionary<string, Search<string>>
                {
                    ["Наименование"] = (node, args) => node
                        .SelectSingleNode(@".//*[@id='appPartZoomCompprd1appPart1zvc_ZoomDetails_ProductBundle__0_0_2_titlerichTextContainer']/p")
                        .InnerText,
                    ["Описание"] = (node, args) => node
                        ?.SelectSingleNode(@".//*[@id='appPartZoomCompprd1appPart1zvc_ZoomDetails_ProductBundle__0_0_2_overviewrichTextContainer']/p")
                        ?.InnerHtml ?? String.Empty + 
                        node
                        ?.SelectSingleNode(@".//*[@id='appPartZoomCompprd1appPart1zvc_ZoomDetails_ProductBundle__0_0_2_detailsrichTextContainer']/p")
                        ?.InnerHtml ?? String.Empty,
                    ["Цена"] = (node, args) => node
                        .SelectSingleNode(@".//*[@id='appPartZoomCompprd1appPart1zvc_ZoomDetails_ProductBundle__0_0_2_pricerichTextContainer']/p")
                        .InnerText.Replace("руб.", String.Empty).Replace(",", String.Empty),
                },
                pluralPropertiesProduct: new Dictionary<string, Search<string[]>>
                {
                    ["Изображения"] = (node, args) => node
                        ._SelectNodes(@"//div[@class='s_xUSelectableSliderGalleryDefaultSkinimageItem']//img")
                        .Select(x => x.Attributes["src"].Value)
                        .Select(x => x.Substring(0, x.IndexOf("v1")))
                        .ToArray()
                }
                );
            var argument = new ArgumentObject(
                url: @"http://www.td-dvoriki.com/toy-shop-cjg9",
                args: new object[] { 0 });

            var collection = parser.GetProductOrCategory(argument);
            Import.Write(path: "tddvoriki.csv",
                collection: collection,
                headers: Constants.WebAsystKeys,
                format: Constants.WebAsystFormatter);
        }
    }
}
