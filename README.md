# Tharga Reporter
Create PDF and send documents to printer using templates and data.

https://www.nuget.org/packages/Tharga.Reporter/


## Brief
The purpose of Tharga Reporter is to create documents like invoices, delivery notes, orders, forms, lables or other types of reports that contains much variable data. It has been used for many years creating all reports for the cash register and invoceing system [Florida](http://florida.thargelion.net).
The reason for me to put the source on github is so that others can make use of it, and improve it if required.

## Document Structure
A document *template* is structured by creating several *sections*. Each *section* can have different layout with *margins*, *pane*, *header* and *footer*. The pane is where the main content will be. The content of the *header* and *footer* can be specified once withing the *section*, or between sections if desired.

### Template
The template is the highest object in the hierarchy. The template needs to contain at least one section, but can contain several sections. Each section is like an individual template. Typically there can be a section for the cover page, one for the content and one for the appendix.

### Section
A section describes the layout of the page. If there are elements spanning over several pages, all pages will look like the section specification. The setup is the margin on all sides, a header and a footer where the content will be the same on all pages of the section, and the pane where the content will flow.

##### Margins
The margin is set as a rectangle of *UnitValues* describing all four sides.

##### Header
The header is placed within the margins on top of the page.
The only value that can be set for this element is the height. It is possible to add elements here, they will appear the same on all pages of the section.
This is a good place to put text that contains special keywords like {Title}.

##### Footer
The footer is placed within the margins on at the bottom of the page.
The only value that can be set for this element is the height. It is possible to add elements here, they will appear the same on all pages of the section.
This is a good place to put text that contains special keywords like {PageNumber} and {TotalPages}.

##### Pane
This is where the content is placed. You cannot set the size of the pane, it gets the space left after you have set the margins, header and footer.
The actual size is created when the document is rendered.
Some elements have the ability to expand over several pages. The elements that does this are *TextArea* and *Table*.

### Types of elements
There are several different types of elements that can be printed. There are *Text*, *TextBox*, *Line*, *Rectangle*, *Image*, *Table*, *BarCode* and *ReferencePoint*.

### Unit Values
A unit value is a value that is implicitly converted from a string to an actual value. You can enter the following types of units for a *unit value*.
- mm - Translates to millimeter on the document
- cm - Translates to centimeter on the document
- in (or inch) inces on the document
- px (default if there is no unit) - Raw value
- % - Percentage part of the current pane.

### Common properties for all element
All elements have got the properties Top, Left, Name, IsBackground and Visibility.
- Top - The location in 'Unit value' from the top where the element is to be placed. This is calculated from the top of the parent wher the element is added, it could be directly on a pane or on a reference point. Default is zero.
- Left - The location in 'Unit value' from the left where the element is to be placed. Default is zero.
- Name - Used for debuging and easy to read serialization.
- IsBackground - If true, the element is only rendered if background objects are requested. When printing on pre, printed documents logotypes and addresses should not appear, but they should appear when creating a PDF, in that case they can be marked as background objects.
- Visibility - This property can be set to specify on what pages the element should be visible. Perhaps it should only appear on the first or the last page of the document. The sum of an invoice might only be required on the last page and the address on the first page, for instance.

### Common properties for area elements
The area elements have some additional properties describing, not just the location, but also the size of the element.
Some of the area elements are *Rectangle*, *Table*, *Text*, *TextBox*, *Image*, *Line* and *BarCode*.

##### Horizontal
Only two of theese properties can be set at once. The third one will be calculated based on the other two. If left out they will all assume default values.

- Top (default 0) - Distance from the top of the pane or parent element (like a reference point).
- Bottom (default 0) - Distance from the bottom of the pane.
- Height - Total height (or proportion of the pane if % is used)

##### Vertical
Only two of theese properties can be set at once. The third one will be calculated based on the other two. If left out they will all assume default values.

- Left (default 0) - Distance from the left side of the pane or parent element.
- Right (default 0) - Distance from the right of the pane.
- Width - Total width (or proportion of the pane if % is used)

### Elements
Here are the elements that can be added to the *header*, *footer* and *pane*. The element *ReferencePoint* also have the ability to be a container for other elements.
In this section only the properties that are specific for each element is described.

##### Text
Text should be used for simple short text that is to appear on all pages in the document. This could be titles or descriptions. Text does not flow over multiple pages, it appears on all pages of the section. Text does not flow over several lines, but you can use *Environment.NewLine* to force it to use more than one line.
You can use special keywords or data variables in texts to make the text dynamic. Read more about this under *Keywords* and *Data*.
If you want a text to appear only on the first or last page of the section there are *page visibility* that can be used.
- Value - This is the data that will be written.
- HideValue - This make it possible for variables to hide data under special conditions. Lets say you have a text saying "${Price}". But, if you provide empty data for {Price}, the $-sign will still show. There is a way of hiding this value. You can do this by setting the HideValue to "{Price}". In this case the {Price} will be empty and it will make the value to be hidden.
- Font - Sets the text to a specific font.
- TextAlignment - This is a horizontal alignment from the point descriped in *Left* and *Right*. The value can be *Left*, *Right* or *Center*.

##### TextBox
TextBox should be used for text that is to flow within a specified area. If the text does not fit within the area a new page will be created on the next page where the text will continue. If there are more text boxes on the samedocument, they will all flow individually and independent of eachother.
It is possible to use keywords and data variables in textboxes, also the special *page visibility* that hides and shows elements on first and last pages are possible to use.
- Value - This is the data that will be written.
- HideValue - This make it possible for variables to hide data under special conditions. Lets say you have a text saying "${Price}". But, if you provide empty data for {Price}, the $-sign will still show. There is a way of hiding this value. You can do this by setting the HideValue to "{Price}". In this case the {Price} will be empty and it will make the value to be hidden.
- Font - Sets the text to a specific font.

##### Line
A line can be drawn between two points of the document. It will appear the same on all pages within the section. The thickness and color of the line can be set as requested.
Rendering the line is always done from the top left to bottom right. Therefore a line cannot be rendered from the bottom left up to the top right. So solve this you can set the *width* or *height* to a negative value.
- Color
- Thickness
- HideValue

##### Rectangle
Rectangles will be drawn the same on all pages within the section, the border color and thickness can be set as well as the background.
- BorderColor
- BorderWidth
- BackgroundColor

##### Image
An image can be loaded from disk or URL and rendered into the document. Images will appear the same on all pages within the section.
When setting the size of the image it can be adjusted proportionate by setting just the height or width. If both are set the image will be stretched to fit within that area.
- Source - The location of the image described as a absolute or relative path (relative to the running application). You can also use a URL to get the image downloaded.
*There is an issue with loading images from URL. You cannot use the char : in the path. (ie. http://localhost:12345/MyImage.png). Feel free to contribute to fixing this issue. :)*

##### Table
Tables are the most complex object to be rendered. The use of tables is what actually drove this project from the beginning. The layout can be set in many different ways. Columns can be added and data from a data document placed into the cells. Tables will flow over several pages.
It is possible to hide columns using different directives depending on the data. For intance, if there is no data in a column it can be hidden from the table.

Theese are the properties that can be set for a table. The header is the *title* part of the table, the *data* are the normal lines. You can also add *grouping* to the table having different sections of *data*.
- HeaderFont
- HeaderBorderColor
- HeaderBackgroundColor
- ContentFont
- ContentBorderColor
- ContentBackgroundColor
- GroupFont
- GroupBorderColor
- GroupBackgroundColor
- GroupSpacing - Extra height for the grouping line
- SkipLine - Skips a sertain *height* every number of rows making tables easier to read.
- RowPadding - Adds extra padding on all rows
- ColumnPadding - Adds extra padding to the columns

###### Table column
Columns are a little bit old-school. This code should better be refactored since it does not follow the pattern of everything else here. The way columns are added to a table really does not make any sense...
- DisplayName - The text that appears in the header for the column.
- Width - Width of the column. If left out is is calculated depending on the *WidthMode*.
- WidthMode - Can be set to *Specific* (the Width has to be set), *Auto* (Automatically calculated in some magic way) and *Spring* (There can be only one in each table. It takes up as much space as it can.)
- Align - Can be set to *Left* or *Right* and affects data and header in the same way.
- HideValue - Value that if rendered empty, collapses the entire column from the table.

Now, where is the data for the *cells* you might wonder. Me too when I wrote this document. It so happens that there is an extra property tha cannot be seen here. It only exists when adding the column to the table.
There is a function called *AddColumn* on the table object. The first parameter of this function is the text that will be used on all *cells*. Use the *data* pattern when adding to this table (ie. "{NameOfData}").
*This should be rewritten so that a table column is created separatley with all its properties and added to the table. This is how all the other elements work.*

###### Table data
I was going to refer to the *Data* section of this document, but since the data of the table so essensial, I decided to put it here. However, you need an understanding of how data works to fully understand this. So it is not a bad idea to read about the *data* section first anyway.
Table data is a special part of the document data. The first thing you need to do is to create a DocumentDataTable with the same name as the table. Table has to have a name so that data can be linked to them.
After that rows can be added to the table data. Use the method AddRow, it takes a dictionary, where the key is the name of the data field of the table (ie. .AddRow(new Dictionary<string, string> { { "NameOfData", "Cell data" } });)

To get a better understanding look at the example class *CreateTableCommand*.

##### BarCode
Barcodes of type *Code39* can be rendered onto the document. They will be the same on all pages of the document within the section.
- Code - The value of the barcode

##### ReferencePoint
A reference point can be used to more easily group elements together so that they can be moved all att once.
Elements on a reference point can be stacked *Vertical* or not att all (absolute positioned).
- Stack - Elements will be stacked over eachother. For instance Text elements under eachother. Or use an absolute position from the reference point.
- ElementList - Elements that belongs to the referebce point.
- HideValue - All elements in the ElementList are hidden using this value.

### Fonts
Fonts have the following properties.

- FontName
- Size
- Color
- Bold
- Italic
- Underline
- Strikeout

### Keywords
There are some keywords that can be used in texts. They are written the same way as data is (ie. {PageNumber}).

- PageNumber - The number of the page
- TotalPages - The total number of pages
- Author - Takes the author value from the document data.
- Subject - Takes the subject value from the document data.
- Title - Takes the title value from the document data.
- Creator - Takes the creator value from the document data.

### PageVisibility

- All - The element shows on all pages (this is default).
- FirstPage - Element appears on the first page only.
- LastPage - Element appears on the last page only.
- AllButFirst - Element appears on all pages but on the last one.
- AllButLast - Element appears on all pages but the first one.
- WhenMultiplePages - Element appears on all pages when there are multiple palges. (This is a great way of showing page number when there are more than one page only)
- WhenSinglePage - Element appears only if there is a single page in the document.

### Data
One of the main feature of Tharga Reporter is the possibility to append data. The whole idea is to create a template where data can be rendered.
There is a document type called *DocumentData* it holds two kinds of data. Data for tables that I will not write about here, there is information about that in the table section.
The regular data is ordered like a key value pair. Add data lile this...

```
var data = new DocumentData();
data.Add("SomeData", "some dynamic data");
```

To get access to the data from the template you can use it from a *text* element if you want to...

```
var text = new Text { Value = "Some static data and {SomeData}." };
```

When the template and the data is put together for rendering the text in this case will read "Some static data and some dynamic data.".

### Serialze and deserialize the document
An important part of the template is that it can be serialized and stored. The easiest way to generate it is to use code, but this is probably not how you want to do it all the time.
You can generate a default temlpate by code, then serialize it and make customizations for different customers.

Use the *.ToXml()* function on the template object to get xml data. To load it use the static creator method *var template = Template.Load(xml);*.

### Rendering
Rendering is what you do to create the actual document. This is where the template and data comes togehter. This is where you specify the size of the document, like A4 or Letter.

There is a *Renderer* object that you will have to create...

```
var renderer = new Renderer(template, documentData, true, documentProperties, debug);
```

The parameters to send to the constructor is
- template - The actual template
- documentData - The data part
- includeBackgroundObjects - If set to true, all elements with the property *IsBackground=true* will be rendered.
- documentProperties - Properties for the document.
- debug - Lines and information will be outputet (in blue), so that you can see what is going on.

##### DocumentProperties
Properties used for PDF documents.
- Author
- Subject
- Title
- Creator

##### IncludeBackgroundObjects
If you have pre-printed papers with logotypes and address information and want to use the same template for PDF documents and print documents this is a great feature.
All objects that appear on the pre-printed paper (as letter heads) should be included in the template and set to *IsBackground=true*. Then you can generate PDF documents with background object and have the logotype there.
Still you can print documents without the background objects on the pre-printed paper. Or, print documents with background on white paper.

When you have your renderer you can choose if you want it created as s PDF or send it to the printer directly.

##### As a PDF
There are two ways of doing this, ither just save it as a pdf file on disk. (ie. renderer.CreatePdfFile("MyDocument.pdf");)
If you want to send the pdf from a server to a client or email it you can get the binary data for the pdf instead. (ie. var bytes = renderer.GetPdfBinary();)

You can provide a parameter with what *PageSize* you want to generate for the pdf.

##### To the printer
When sending the document to a printer the regular printer settings object is used. If you want to use the default printer directly just create a new object (ie. new PrinterSettings { Copies = 1 }).
You can also use the common windows dialog to get the object...

```
var dialog = new PrintDialog
{
    UserPageRangeEnabled = false,
    SelectedPagesEnabled = false,
};

if (dialog.ShowDialog() == true)
{
    var printerSettings = new PrinterSettings
    {
        Copies = (short) (dialog.PrintTicket.CopyCount ?? 1),
        PrinterName = dialog.PrintQueue.FullName,
    };
}
```

Then just execute the *Print* function. (ie renderer.Print(printerSettings);)

## Some technical stuff
To generate the PDF documents *PDFsharp* is used and for printing we use *MigraDoc*, and to generate the barcodes *Aspose.BarCode*. They are all referenced as nuget packages.

### Pre-rendering and rendering
One thing that is good to know when working with this project is how the renderer works.
If you want to change something it is most likely a new type of element or a new property on an existing element. Adding properties to object is easy. Then you will have to change how it is outputed, that means updating the renderer.
Since the renderer is the one that knows about sizes of things, it is the one that has to calculate the number of pages. Therefore the first thing done is a pre-rendering that just does the calculation, but it does not output anything. After that the actual rendering is started.

## Finally
This tool is used professionaly by several projekts so it works for something. It is not the prettiest code so feel free to clean it up. :)
There are some tests assuring quality and help protecting when refactoring.

The thing I like the least is that the renderer holds state between the pre-rendering and actual rendering. This is fairly hard to change and makes for big impact. I have some ideas on how it could be, but I have not had the time to change it yet.

## Future
Some things that is next in line to be fixed is custom document sizes. I want to be able to print lables for a project that I work with now.
Also it would be nice to make use of all PDF properties, like readonly flags and password protection.

## License
Tharga Reporter goes under The MIT License (MIT).