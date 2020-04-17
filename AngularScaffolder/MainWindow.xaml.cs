using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AngularScaffolder.Extensions;
using AngularScaffolder.Objects;
using Microsoft.Win32;

namespace AngularScaffolder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == true)
            {
                SourceClassTextBox.Text = dialog.FileName;
            }
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if(File.Exists(SourceClassTextBox.Text))
            {
                var entityInfo = ParseEntity(SourceClassTextBox.Text);

                var sb = new StringBuilder();

                if (InterfaceCheckBox.IsChecked == true)
                {
                    sb = CreateInterfaceCode(sb, entityInfo);
                }

                if (FormCheckBox.IsChecked == true)
                {
                    sb = CreateFormConstantCode(sb, entityInfo);
                }

                if (ColumnsCheckBox.IsChecked == true)
                {
                    sb = CreateColumnConstantsAndTableCode(sb, entityInfo);
                }

                if(ServiceCheckBox.IsChecked == true)
                {
                    sb = CreateServiceCode(sb, entityInfo);
                }

                OutputTextBox.Text = sb.ToString();
            }
        }

        private StringBuilder CreateServiceCode(StringBuilder sb, EntityInfo info)
        {
            sb.AppendLine("import { BaseApi } from './base-api';");
            sb.AppendLine("import { Injectable } from '@angular/core';");
            sb.AppendLine("import { HttpClient, HttpHeaders } from '@angular/common/http';");
            sb.AppendLine("import { Observable } from 'rxjs';");
            sb.AppendLine("import { EnvironmentManager } from '../configuration/environment-manager';");
            sb.AppendLine("import { tap, catchError } from 'rxjs/operators';");
            sb.AppendLine($"import {{ {info.ClassName} }} from '../models/{info.AngularModelName}.model';");
            sb.AppendLine();
            sb.AppendLine("const httpOptions = {");
            sb.AppendLine("\theaders: new HttpHeaders({'Content-Type': 'application/json'})");
            sb.AppendLine("};");
            sb.AppendLine();
            sb.AppendLine($"const api = '/api/{info.ClassName}';");
            sb.AppendLine();
            sb.AppendLine("@Injectable({");
            sb.AppendLine("\tprovidedIn: 'root'");
            sb.AppendLine("})");
            sb.AppendLine($"export class {info.ClassName}ApiService extends BaseApi {{");
            sb.AppendLine("\tbaseUri = EnvironmentManager.getApiUrl();");
            sb.AppendLine();
            sb.AppendLine("\tconstructor(private http: HttpClient) { super(); }");
            sb.AppendLine();

            sb.AppendLine($"\tgetAll(): Observable<{info.ClassName}[]> {{");
            sb.AppendLine("\t\tconst url = `${this.baseUri}${api}`;");
            sb.AppendLine();
            sb.AppendLine($"\t\treturn this.http.get<{info.ClassName}[]>(url)");
            sb.AppendLine("\t\t\t.pipe(");
            sb.AppendLine($"\t\t\t\ttap(_ => console.log('fetched {info.ClassName.ToLower()}s')),");
            sb.AppendLine($"\t\t\t\tcatchError(this.handleError('{info.ClassName}ApiService.getAll', []))");
            sb.AppendLine("\t\t\t);");
            sb.AppendLine("\t}");
            sb.AppendLine();

            sb.AppendLine($"\tget(id: number): Observable<{info.ClassName}> {{");
            sb.AppendLine("\t\tconst url = `${this.baseUri}${api}/${id}`;");
            sb.AppendLine();
            sb.AppendLine($"\t\treturn this.http.get<{info.ClassName}>(url)");
            sb.AppendLine("\t\t\t.pipe(");
            sb.AppendLine($"\t\t\t\ttap(_ => console.log('fetched {info.ClassName.ToLower()} with id of ${{id}}')),");
            sb.AppendLine($"\t\t\t\tcatchError(this.handleError(`{info.ClassName}ApiService.get id=${{id}}`))");
            sb.AppendLine("\t\t\t);");
            sb.AppendLine("\t}");
            sb.AppendLine();

            sb.AppendLine($"\taddNew({info.ClassName.ToCamelCase()}: {info.ClassName}): Observable<{info.ClassName}> {{");
            sb.AppendLine("\t\tconst url = `${this.baseUri}${api}`;");
            sb.AppendLine();
            sb.AppendLine($"\t\treturn this.http.post<{info.ClassName}>(url, {info.ClassName.ToCamelCase()}, httpOptions)");
            sb.AppendLine("\t\t\t.pipe(");
            sb.AppendLine($"\t\t\t\ttap((data: {info.ClassName}) => {{");
            sb.AppendLine($"\t\t\t\t\tconsole.log('added new {info.ClassName.ToLower()}');");
            sb.AppendLine("\t\t\t\t\tconsole.log(data);");
            sb.AppendLine("\t\t\t\t});");
            sb.AppendLine($"\t\t\t\tcatchError(this.handleError(`{info.ClassName}ApiService.addNew`))");
            sb.AppendLine("\t\t\t);");
            sb.AppendLine("\t}");
            sb.AppendLine();

            sb.AppendLine($"\tupdate(id: number, {info.ClassName.ToCamelCase()}: {info.ClassName}): Observable<any> {{");
            sb.AppendLine("\t\tconst url = `${this.baseUri}${api}/${id}`;");
            sb.AppendLine();
            sb.AppendLine($"\t\treturn this.http.put(url, {info.ClassName.ToCamelCase()}, httpOptions)");
            sb.AppendLine("\t\t\t.pipe(");
            sb.AppendLine($"\t\t\t\ttap(_ => console.log('updated {info.ClassName.ToLower()} with id of ${{id}}')),");
            sb.AppendLine($"\t\t\t\tcatchError(this.handleError('{info.ClassName}ApiService.update'))");
            sb.AppendLine("\t\t\t);");
            sb.AppendLine("\t}");
            sb.AppendLine();

            sb.AppendLine($"\tdelete(id: number): Observable<{info.ClassName}> {{");
            sb.AppendLine("\t\tconst url = `${this.baseUri}${api}/${id}`;");
            sb.AppendLine();
            sb.AppendLine($"\t\treturn this.http.delete(url, httpOptions)");
            sb.AppendLine("\t\t\t.pipe(");
            sb.AppendLine($"\t\t\t\ttap(_ => console.log('deleted {info.ClassName.ToLower()} with id of ${{id}}')),");
            sb.AppendLine($"\t\t\t\tcatchError(this.handleError('{info.ClassName}ApiService.delete'))");
            sb.AppendLine("\t\t\t);");
            sb.AppendLine("\t}");

            sb.AppendLine("}");

            sb.AppendLine().AppendLine();

            return sb;
        }

        private StringBuilder CreateColumnConstantsAndTableCode(StringBuilder sb, EntityInfo info)
        {
            var sbTable = new StringBuilder();

            sb.AppendLine("columnNames = {");

            sbTable.AppendLine("<div class=\"fixed-container\">");
            sbTable.AppendLine("\t<table mat-table [dataSource]=\"dataSource\" class=\"mat-elevation-z8\">");

            foreach (var property in info.Properties)
            {
                sb.AppendLine($"\t{property.Name}: '{property.CamelCaseName}',");

                sbTable.AppendLine($"\t\t<ng-container [matColumnDef]=\"columnNames.{property.Name}\">");
                sbTable.AppendLine($"\t\t\t<th mat-header-cell *matHeaderCellDef>{property.NameWithSpaces}</th>");
                sbTable.AppendLine($"\t\t\t<td mat-cell *matCellDef=\"let row\"> {{{{ row[columnNames.{property.Name}] }}}} </td>");
                sbTable.AppendLine("\t\t</ng-container>");
            }

            sb.Remove(sb.Length - 1, 1);

            sbTable.AppendLine("\t\t<tr mat-header-row *matHeaderRowDef=\"displayedColumns; sticky: true\"></tr>");
            sbTable.AppendLine("\t\t<tr mat-row *matRowDef=\"let row; columns: displayedColumns; \"></tr>");
            sbTable.AppendLine("\t</table>");
            sbTable.AppendLine("\t<mat-card *ngIf=\"isLoading\"");
            sbTable.AppendLine("\t\tstyle=\"display: flex; justify - content: center; align - items: center\">");
            sbTable.AppendLine("\t\t <mat-progress-bar mode=\"indeterminate\"></mat-progress-bar>");
            sbTable.AppendLine("\t</mat-card>");
            sbTable.AppendLine("\t<mat-card *ngIf=\"noRecordsReturned\"");
            sbTable.AppendLine("\t\tstyle=\"display: flex; justify-content: center; align-items: center\">");
            sbTable.AppendLine("\t\tNo Data Found for the Filters you selected.");
            sbTable.AppendLine("\t</mat-card>");
            sbTable.AppendLine("</div>");

            sb.AppendLine("};");
            sb.AppendLine("displayedColumns = [...Object.values(this.columnNames)];");
            sb.AppendLine().AppendLine();

            sb.Append(sbTable.ToString());

            sb.AppendLine().AppendLine();

            return sb;
        }

        private StringBuilder CreateFormConstantCode(StringBuilder sb, EntityInfo info)
        {
            sb.AppendLine("formControls = {");

            foreach (var property in info.Properties)
            {
                sb.AppendLine($"\t{property.Name}: '{property.CamelCaseName}',");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine("};");

            sb.AppendLine().AppendLine();
            return sb;
        }

        private StringBuilder CreateInterfaceCode(StringBuilder sb, EntityInfo info)
        {
            sb.AppendLine($"export interface {info.ClassName} {{");

            foreach (var property in info.Properties)
            {
                sb.AppendLine($"\t{property.CamelCaseName}: {property.DataType};");
            }

            sb.AppendLine("}");
            sb.AppendLine().AppendLine();
            return sb;
        }

        private EntityInfo ParseEntity(string filePath)
        {
            //var properties = new List<(string Name, string DataType, string originalName)>();
            var info = new EntityInfo();
            var propertyPattern = @"public .+? \w+? { get; set; }";
            var classIdentifier = "public class ";
            var classPattern = "public(.+?| )class";
            var regexProperty = new Regex(propertyPattern);
            var regexClass = new Regex(classPattern);

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (regexClass.IsMatch(line))
                    {
                        int endOfClassName = 0;
                        int startOfClassName = line.IndexOf(classIdentifier) + classIdentifier.Length;

                        if (line.Contains(":"))
                        {
                            endOfClassName = line.IndexOf(":") - 1 - startOfClassName;
                        }
                        else
                        {
                            endOfClassName = line.Length - startOfClassName;
                        }

                        info.ClassName = line.Substring(startOfClassName, endOfClassName);
                    }
                    else if (regexProperty.IsMatch(line))
                    {
                        var lineParts = line.Trim().Split(' ');
                        var dataType = lineParts[1];
                        var property = lineParts[2];
                        var camelCaseProperty = $"{property.Substring(0, 1).ToLower()}{property.Substring(1)}";
                        info.Properties.Add(new PropertyInfo
                            {
                                Name = property,
                                CamelCaseName = camelCaseProperty,
                                DataType = dataType
                            });
                    }
                }
            }

            return info;
        }
    }
}
