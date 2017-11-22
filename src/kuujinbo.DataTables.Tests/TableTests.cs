using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.DataTables.Tests
{
    #region test models
    public class TestModel : IIdentifier
    {
        public TestModel()
        {
            Hobbies = new List<TestHobby>();
        }

        public int Id { get; set; }

        [Column(DisplayOrder = 1)]
        public string Name { get; set; }

        [Column(DisplayOrder = 2)]
        public string Office { get; set; }

        [Column(DisplayOrder = 3, DisplayName = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Column(DisplayOrder = 4, FieldAccessor = "Amount")]
        public TestSalary Salary { get; set; }

        [Column(DisplayOrder = 5, FieldAccessor = "Name")]
        public ICollection<TestHobby> Hobbies { get; set; }
    }
    public class TestSalary
    {
        public int Amount { get; set; }
    }
    public class TestHobby
    {
        public string Name { get; set; }
    }
    #endregion

    public class TableTests
    {
        #region test class/setup

        Table _table;
        IEnumerable<TestModel> _modelData;
        private readonly ITestOutputHelper output;
        public TableTests(ITestOutputHelper output)
        {
            _modelData = new List<TestModel>() { SATO, RAMOS, GREER, KELLY, ITO };
            this.output = output;
        }

        public static readonly TestModel SATO = new TestModel
        {
            Id = 1,
            Name = "Satou, Airi",
            Office = "Tokyo",
            StartDate = new DateTime(2008, 11, 28),
            Salary = new TestSalary() { Amount = 80000 },
            Hobbies = new List<TestHobby>() 
            { 
                new TestHobby() { Name = "hobby 0"},
                new TestHobby(), 
                new TestHobby() { Name = "  "}
            }
        };
        public static readonly TestModel RAMOS = new TestModel
        {
            Id = 25,
            Name = "Ramos, Angelica",
            Office = "London",
            StartDate = new DateTime(2010, 1, 1),
            Salary = new TestSalary() { Amount = 70000 },
            Hobbies = new List<TestHobby>() 
            { 
                new TestHobby() { Name = "hobby 4"}, 
                new TestHobby() { Name = "hobby 3"}
            }
        };
        public static readonly TestModel GREER = new TestModel
        {
            Id = 20,
            Name = "Greer, Bradley",
            Office = "London",
            Salary = new TestSalary() { Amount = 40000 },
            Hobbies = new List<TestHobby>() 
            { 
                new TestHobby() { Name = "hobby 5"}
            }
        };
        public static readonly TestModel KELLY = new TestModel
        {
            Id = 4,
            Name = "Kelly, Cedric",
            Office = "Edinburgh",
            Salary = new TestSalary() { Amount = 76000 },
            Hobbies = new List<TestHobby>() 
            { 
                new TestHobby() { Name = "hobby 8"}
            }
        };
        public static readonly TestModel ITO = new TestModel
        {
            Id = 30,
            Name = "Itou, Shou",
            // explicitly cover null property check in ExecuteRequest()
            Office = null,
            Salary = new TestSalary() { Amount = 100000 },

        };

        /// <summary>
        /// act and assert for current [Fact]
        /// </summary>
        /// <param name="entities">
        /// entity collection in **EXPECTED** order 
        /// </param>
        /// <remarks>perform arrange in **CURRENT** [Fact]</remarks>
        private void ActAndAssert(params TestModel[] entities)
        {
            // arrange
            int entityCount = entities.Length;
            var totalRecords = _modelData.Count();

            // act
            _table.ExecuteRequest<TestModel>(_modelData);
            var data = _table.Data;

            // assert
            Assert.Equal(totalRecords, _table.RecordsTotal);
            Assert.IsType<List<List<object>>>(data);
            // recordsFiltered != recordsTotal when searching
            Assert.Equal(entityCount, _table.RecordsFiltered);

            for (int i = 0; i < entityCount; ++i)
            {
                Assert.Equal(entities[i].Id, data[i][0]);
                Assert.Equal(entities[i].Name, data[i][1]);
                Assert.Equal(entities[i].Office, data[i][2]);
                Assert.Equal(entities[i].StartDate, data[i][3]);
                Assert.Equal(entities[i].Salary.Amount, data[i][4]);
                Assert.Equal(
                    string.Join(", ", entities[i].Hobbies
                        .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                        .OrderBy(x => x.Name)
                        .Select(x => x.Name)),
                    data[i][5]
                );
                output.WriteLine("{0} hobbies: {1}", data[i][1], data[i][5]);
            }
        }
        #endregion


        #region tests
        [Fact]
        public void ShowCheckboxColumn_NoActionButtons_ReturnsFalse()
        {
            var table = new Table()
            {
                ActionButtons = new List<ActionButton>()
                {
                    new ActionButton("/Create", "Create"),
                    new ActionButton("/Delete", "Delete")
                }
            };

            Assert.Equal(true, table.ShowCheckboxColumn());
        }

        [Fact]
        public void ShowCheckboxColumn_ActionButtons_ReturnsTrue()
        {
            var table = new Table();

            Assert.Equal(false, table.ShowCheckboxColumn());
        }

        [Fact]
        public void SetColumns_WhenCalled_AddsColumnsToTable()
        {
            _table = new Table();

            _table.SetColumns<TestModel>();

            Assert.Equal(5, _table.Columns.Count());
            Assert.Equal("Name", _table.Columns.ElementAt(0).Name);
            Assert.Equal("Office", _table.Columns.ElementAt(1).Name);
            Assert.Equal("Start Date", _table.Columns.ElementAt(2).Name);
            Assert.Equal("Salary", _table.Columns.ElementAt(3).Name);
            Assert.Equal("Hobbies", _table.Columns.ElementAt(4).Name);
        }

        // no sort or search criteria
        [Fact]
        public void GetData_DefaultCall_ReturnsUnsortedCollection()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>()
            };
            _table.SetColumns<TestModel>();

            ActAndAssert(SATO, RAMOS, GREER, KELLY, ITO);
        }

        [Fact]
        public void GetData_SortCriteriaNoSearchCriteria_ReturnsAscendingSort()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>() 
                { 
                    // sort ascending => 'Name' property
                    new SortOrder { ColumnIndex = 0, Direction = ModelBinder.ORDER_ASC } 
                }
            };
            _table.SetColumns<TestModel>();

            ActAndAssert(GREER, ITO, KELLY, RAMOS, SATO);
        }

        [Fact]
        public void GetData_SortNonAscCriteriaNoSearchCriteria_ReturnsDescendingSort()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>() 
                { 
                    // sort ascending => 'Name' property
                    // anything other than ModelBinder.ORDER_ASC is descending 
                    new SortOrder { ColumnIndex = 0, Direction = "anything other than 'asc'" } 
                }
            };
            _table.SetColumns<TestModel>();

            ActAndAssert(SATO, RAMOS, KELLY, ITO, GREER);
        }

        [Fact]
        public void GetData_SearchNullOrEmptyCriteria_IgnoresSearchAndReturnsAllData()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>()
            };
            _table.SetColumns<TestModel>();
            _table.Columns.ElementAt(1).Search = new Search() { Value = "  ", ColumnIndex = 1 };
            _table.Columns.ElementAt(2).Search = new Search() { ColumnIndex = 2 };

            ActAndAssert(SATO, RAMOS, GREER, KELLY, ITO);
        }

        [Fact]
        public void GetData_SearchCriteriaNoSortCriteria_ReturnsSearchMatchInOriginalOrder()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>()
            };
            _table.SetColumns<TestModel>();
            // search 'Name' property => case-insensitive
            _table.Columns.ElementAt(0).Search = new Search() { Value = "g", ColumnIndex = 0 };

            ActAndAssert(RAMOS, GREER);
        }

        [Fact]
        public void GetData_SortAndSearchCriteria_ReturnsSearchMatchInRequestedOrder()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>() 
                { 
                    // sort ascending => 'StartDate' property
                    new SortOrder { ColumnIndex = 2, Direction = ModelBinder.ORDER_ASC },
                    // sort descending => 'Name' property, which should be
                    // ignored, since 'StartDate' is evaluated first
                    new SortOrder { ColumnIndex = 0, Direction = "other" },
                }
            };
            _table.SetColumns<TestModel>();
            // search 'Office' property => case-insensitive
            _table.Columns.ElementAt(1).Search = new Search() { Value = "lon", ColumnIndex = 1 };

            ActAndAssert(GREER, RAMOS);
        }

        [Fact]
        public void GetData_MultiSearchCriteriaNoSortCriteria_ReturnsSearchMatchInOriginalOrder()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>()
            };
            _table.SetColumns<TestModel>();
            // search 'Office' property => case-insensitive
            _table.Columns.ElementAt(1).Search = new Search() { Value = "EdiNBUrgh|LoNDon", ColumnIndex = 1 };

            ActAndAssert(RAMOS, GREER, KELLY);
        }

        [Fact]
        public void GetData_MultiSearchAndSortCriteria_ReturnsSearchMatchInRequestedOrder()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>() 
                { 
                    // sort ascending => 'Salary' property
                    new SortOrder { ColumnIndex = 3, Direction = ModelBinder.ORDER_ASC },
                }

            };
            _table.SetColumns<TestModel>();
            // search 'Salary' property
            _table.Columns.ElementAt(3).Search = new Search() { Value = "40000|80000", ColumnIndex = 3 };

            ActAndAssert(GREER, SATO);
        }

        [Fact]
        public void GetData_SaveAsTrue_ReturnsDataWithColumnNamesDefaultStartDefaultLength()
        {
            _table = new Table()
            {
                SaveAs = true,
                Start = 1000,   // should reset to 0 
                Length = 10     // should reset to default
            };
            _table.SetColumns<TestModel>();

            // act
            _table.ExecuteRequest<TestModel>(_modelData);
            var data = _table.Data;
            var headerRow = data.ElementAt(0);

            // assert
            Assert.Equal(0, _table.Start);
            Assert.Equal(TableSettings.DEFAULT_MAX_SAVE_AS, _table.Length);
            Assert.Equal(5, headerRow.Count());
            Assert.Equal("Name", headerRow.ElementAt(0));
            Assert.Equal("Office", headerRow.ElementAt(1));
            Assert.Equal("Start Date", headerRow.ElementAt(2));
            Assert.Equal("Salary", headerRow.ElementAt(3));
            Assert.Equal("Hobbies", headerRow.ElementAt(4));
        }
        #endregion
    }
}