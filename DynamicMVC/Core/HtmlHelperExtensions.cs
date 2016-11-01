using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicMVC.Core.Enum;

namespace DynamicMVC.Core
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString TableItem(this HtmlHelper htmlHelper, TableOption opt)
        {
            var div = new TagBuilder("table");
            div.Attributes.Add("id", opt.Id);
            div.Attributes.Add("cellspacing", "0");
            div.Attributes.Add("width", "100%");
            div.AddCssClass("dataTables");
            div.AddCssClass("display");
            div.AddCssClass("hover");
            div.AddCssClass("order-column");
            div.AddCssClass("stripe");

            if (opt.Data.Columns.Count < (opt.Orders?.Count() ?? 0)) throw new IndexOutOfRangeException("The Orders columns numbers is more than data table columns count!");
            if (opt.Data.Columns.Count < (opt.CurrencyColumns?.Count() ?? 0)) throw new IndexOutOfRangeException("The Currency columns numbers is more than data table columns count!");
            if (opt.Data.Columns.Count < (opt.AverageFooterColumns?.Count() ?? 0)) throw new IndexOutOfRangeException("The Average columns numbers is more than data table columns count!");
            if (opt.Data.Columns.Count < (opt.TotalFooterColumns?.Count() ?? 0)) throw new IndexOutOfRangeException("The Sum and Total columns numbers is more than data table columns count!");

            var schema =
                opt.Data.Columns.Cast<DataColumn>()
                    .Select(c => new
                    {
                        Name = c.ColumnName,
                        Header = Resources.Localization.ResourceManager.GetString(c.ColumnName) ?? c.ColumnName
                    }).ToList();

            if (opt?.Orders?.Any() == true)
            {
                var val = "[";
                foreach (Tuple<int, OrderType> order in opt.Orders)
                {
                    var targetColIndex = order.Item1 + (opt.Checkable ? 1 : 0);
                    val += $@"[ {targetColIndex}, ""{order.Item2}"" ], ";
                }
                val = val.Substring(0, val.Length - 2) + "]";

                div.Attributes.Add("data-order", val);
            }

            var thHeader = opt.Checkable ? $"<th class='dt-body-center sorting_disabled'><input name='select_all' value='1' type='checkbox'></th>{Environment.NewLine}" : "";
            var thFooter = opt.Checkable ? $"<th></th>{Environment.NewLine}" : "";
            //
            // set sum footer columns
            for (var colIndex = 0; colIndex < schema.Count; colIndex++)
            {
                string classification = "";
                //
                // check is total column or not!
                if (opt.TotalFooterColumns != null)
                {
                    foreach (var colName in opt.TotalFooterColumns)
                    {
                        int index;
                        if (colName.Equals(schema[colIndex].Name, StringComparison.CurrentCultureIgnoreCase)
                            || (int.TryParse(colName, out index) && index == colIndex))
                        {
                            classification = "sum";
                            break;
                        }
                    }
                }
                //
                // check is average column or not!
                if (opt.AverageFooterColumns != null)
                {
                    foreach (var colName in opt.AverageFooterColumns)
                    {
                        int index;
                        if (colName.Equals(schema[colIndex].Name, StringComparison.CurrentCultureIgnoreCase)
                            || (int.TryParse(colName, out index) && index == colIndex))
                        {
                            classification = "avg";
                            break;
                        }
                    }
                }

                classification = string.IsNullOrEmpty(classification) ? "empty" : classification;
                var headFoot = $"<th class='{classification}' style='text-align:left'>{schema[colIndex].Header}</th>{Environment.NewLine}";
                thHeader += headFoot;
                thFooter += headFoot;
            }

            var header = $@"
                            <thead>
                                <tr>
                                   {thHeader}
                                </tr>
                            </thead>
                            ";

            var footer = $@"<tfoot>
                              <tr>{thFooter}</tr>
                            </tfoot>
                            ";

            //
            // check which columns is input!
            if (opt.InputColumnsDataMember.Any())
            {
                var keys = opt.InputColumnsDataMember.Keys.ToArray();
                foreach (string key in keys)
                {
                    // if key is column index then find that name and set again by name and combo option data
                    int index;
                    if (int.TryParse(key, out index))
                    {
                        if (index >= schema.Count) throw new IndexOutOfRangeException("The InputColumnsDataMember has index out of schema columns index range!");
                        opt.InputColumnsDataMember[schema[index].Name] = opt.InputColumnsDataMember[key]; // get column name of found index
                    }
                }
            }

            var trSelectCheckableClass = opt.Checkable ? "" : "notCheckable";
            var tRows = "";
            for (var rIndex = 0; rIndex < opt.Data.Rows.Count; rIndex++)
            {
                var row = opt.Data.Rows[rIndex];

                var tds = opt.Checkable ? $@"<td id='{opt.Id}_colSelect_{rIndex}' class='colSelect' style='text-align: center;'><input id='row' type='checkbox' value='false'></td>{Environment.NewLine}" : "";
                foreach (var col in schema)
                {
                    if (opt.InputColumnsDataMember.ContainsKey(col.Name))
                    {
                        var input = opt.InputColumnsDataMember[col.Name];
                        var data = "";
                        if (input is ComboBoxOption)
                        {
                            data = htmlHelper.ComboBox((ComboBoxOption)input).ToHtmlString();
                        }
                        else if (input is TextBoxOption)
                        {
                            data = htmlHelper.TextBox((TextBoxOption)input, row[col.Name].ToString()).ToHtmlString();
                        }
                        tds += $"<td>{data}</td>{Environment.NewLine}";
                    }
                    else
                    {
                        tds += $"<td>{row[col.Name]}</td>{Environment.NewLine}";
                    }
                }
                tRows += $"<tr class='{trSelectCheckableClass}'>{Environment.NewLine}{tds}{Environment.NewLine}</tr>";
            }

            var body = $"{header}{footer} <tbody>{Environment.NewLine}{tRows}{Environment.NewLine}</tbody>";

            div.InnerHtml = body;

            return MvcHtmlString.Create(div.ToString());
        }

        public static MvcHtmlString ComboBox(this HtmlHelper htmlHelper, ComboBoxOption opt)
        {
            if (opt == null) throw new ArgumentNullException("opt");

            var div = new TagBuilder("select");
            div.Attributes.Add("id", opt.Id);
            div.AddCssClass("selectpicker");
            if (opt.ShowTick) div.AddCssClass("show-tick");
            if (opt.ShowMenuArrow) div.AddCssClass("show-menu-arrow");
            if (opt.EnforceDesiredWidths) div.AddCssClass("form-control");

            if (opt.DataStyle != DataStyleType.none) div.Attributes.Add("data-style", $"btn-{opt.DataStyle.ToString()}");
            if (opt.DataLiveSearch) div.Attributes.Add("data-live-search", "true");
            if (opt.MultipleSelection)
            {
                div.Attributes.Add("multiple", null);
                if (!string.IsNullOrEmpty(opt.MultipleSelectedTextFormat)) div.Attributes.Add("data-selected-text-format", opt.MultipleSelectedTextFormat);
                div.Attributes.Add("data-max-options", opt.DataMaxOptions?.ToString() ?? "auto");
            }
            if (!string.IsNullOrEmpty(opt.Placeholder)) div.Attributes.Add("title", opt.Placeholder);
            if (!string.IsNullOrEmpty(opt.DataWidth)) div.Attributes.Add("data-width", opt.DataWidth);
            if (!string.IsNullOrEmpty(opt.DataSize)) div.Attributes.Add("data-size", opt.DataSize);
            if (opt.ShowSelectDeselectAllOptionsBox) div.Attributes.Add("data-actions-box", "true");
            if (!string.IsNullOrEmpty(opt.MenuHeaderText)) div.Attributes.Add("data-header", opt.MenuHeaderText);
            if (!opt.Enable) div.Attributes.Add("disabled", null);
            div.Attributes.Add("data-show-subtext", opt.ShowOptionSubText.ToString().ToLower());

            var body = "";

            foreach (var data in opt.Data)
            {
                if (data.IsDividerLine) body += "<option data-divider='true'></option>";
                else
                {
                    var subtext = string.IsNullOrEmpty(data.SubText) ? "" : $"data-subtext='{data.SubText}'";
                    body += $"<option value='{data.Value}' {subtext}>{data.Text}</option>";
                }
            }

            div.InnerHtml = body;

            return MvcHtmlString.Create(div.ToString());
        }

        public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, TextBoxOption opt, string value = null)
        {
            if (opt == null) throw new ArgumentNullException("opt");

            var div = new TagBuilder("div");
            var input = new TagBuilder("input");
            input.Attributes.Add("id", opt.Id);
            input.Attributes.Add("autocomplete", opt.AutoComplete ? "on" : "off");
            if (opt.AutoFocus) input.Attributes.Add("autofocus", null);
            if (!string.IsNullOrEmpty(opt.Name)) input.Attributes.Add("name", opt.Name);
            if ((opt.Type == InputTypes.Checkbox || opt.Type == InputTypes.Radio) && opt.Checked) input.Attributes.Add("checked", null);
            if (opt.Max != null) input.Attributes.Add("max", opt.Max.ToString());
            if (opt.Min != null) input.Attributes.Add("min", opt.Min.ToString());
            if (opt.Step != null) input.Attributes.Add("step", opt.Step.ToString());
            if (opt.ReadOnly) input.Attributes.Add("readonly", null);
            input.Attributes.Add("type", opt.Type);
            var val = opt.Value ?? value;
            if (val != null && val != opt.DefaultValue) input.Attributes.Add("value", val);
            input.Attributes.Add("placeholder", opt.Placeholder);
            if (!opt.Enable) input.Attributes.Add("disabled", null);
            input.AddCssClass("form-control");
            if (opt.Type == InputTypes.File) input.AddCssClass("form-control-file");
            if (opt.Type == InputTypes.Radio) input.AddCssClass("radio");
            if (opt.Type == InputTypes.Checkbox) input.AddCssClass("checkbox");
            if (opt.DataStyle != DataStyleType.none)
            {
                input.AddCssClass("form-control-" + opt.DataStyle.ToString().ToLower());
                div.AddCssClass("has-" + opt.DataStyle.ToString().ToLower());
            }

            div.InnerHtml = input.ToString();

            return MvcHtmlString.Create(div.ToString());
        }
    }
}