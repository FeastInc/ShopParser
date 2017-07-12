using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;

namespace DataParser
{
    public class Constants
    {
        public static string[]
            WebAsystKeys = (@"Наименование;""Наименование артикула"";""Код артикула"";Валюта;Цена;" +
                                        @"""Доступен для заказа"";""Зачеркнутая цена"";""Закупочная цена"";" +
                                        @"""Краткое описание"";Описание;" +
                                        @"Теги;Заголовок;""Тип товаров""" +
                                        @"""META Keywords"";""META Description"";""Ссылка на витрину"";" +
                                        @"Производитель;Объем;Размеры;" + 
                                        @"""Возраст детей"";Страна-производитель;Материал").Split(';');

        public static Func<string, string> WebAsystFormatter =
            s => $"\"{WebUtility.HtmlDecode(s.Trim()).Replace('"', '\'')}\"";

        public static byte[] BOM = {239, 187, 191};

    }
}
