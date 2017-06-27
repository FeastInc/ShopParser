using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public class Constants
    {
        public static string[] 
            WebAsystKeys = (@"Наименование;""Наименование артикула"";""Код артикула"";Валюта;Цена;" +
                                        @"""Доступен для заказа"";""Зачеркнутая цена"";""Закупочная цена"";" +
                                        @"""Краткое описание"";Описание;" +
                                        @"Теги;Заголовок;" +
                                        @"""META Keywords"";""META Description"";""Ссылка на витрину"";" +
                                        @"Производитель;Объем;Изображения").Split(';');

    }
}
