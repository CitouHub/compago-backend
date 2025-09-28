using Compago.Domain.ExternalSourceExample.GSuite;
using Compago.Domain.ExternalSourceExample.MicosoftAzure;

namespace Compago.Service.ExternalSource
{
    public static class ExampleResponse
    {
        public static Data GetExample()
        {
            return new()
            {
                FinancialInfo = new()
                {
                    Currency = "USD",
                    InvoiceDescritions = [
                        new() {
                            Id = "563ba512-3a24-492b-8b04-c44b687b1538",
                            Cost = 152.16,
                            InvoiceDate = new DateTime(2025, 01, 01)
                        },
                        new() {
                            Id = "04d856fd-1ec6-4038-8f9e-3a94bffc4fc6",
                            Cost = 142.45,
                            InvoiceDate = new DateTime(2025, 02, 01)
                        },
                        new() {
                            Id = "9a690d5f-8fcf-4e5d-8b66-1d9dfe8e4c5c",
                            Cost = 157.26,
                            InvoiceDate = new DateTime(2025, 03, 01)
                        },
                        new() {
                            Id = "bd1686b6-f591-4be5-9caf-451967118a62",
                            Cost = 152.56,
                            InvoiceDate = new DateTime(2025, 04, 01)
                        },
                        new() {
                            Id = "c90c9df8-2d50-47a4-9801-679fbc1896f9",
                            Cost = 151.18,
                            InvoiceDate = new DateTime(2025, 05, 01)
                        },
                        new() {
                            Id = "d33495b3-a5ee-4cc8-a042-c04ce77677c7",
                            Cost = 156.39,
                            InvoiceDate = new DateTime(2025, 06, 01)
                        },
                        new() {
                            Id = "23fb804a-d12d-4a49-bbd2-c540677c5b79",
                            Cost = 153.33,
                            InvoiceDate = new DateTime(2025, 07, 01)
                        },
                        new() {
                            Id = "b0fae954-415b-43f7-91a4-fe2c2a078c31",
                            Cost = 152.73,
                            InvoiceDate = new DateTime(2025, 08, 01)
                        },
                        new() {
                            Id = "e4ca6de3-713b-428a-bf29-d74814e61417",
                            Cost = 151.11,
                            InvoiceDate = new DateTime(2025, 09, 01)
                        },
                        new() {
                            Id = "a76f7308-17ce-43f8-aae2-74a96f267f20",
                            Cost = 153.60,
                            InvoiceDate = new DateTime(2025, 10, 01)
                        },
                        new() {
                            Id = "a49904c6-bc98-40de-ac85-0f8e45f42605",
                            Cost = 152.54,
                            InvoiceDate = new DateTime(2025, 11, 01)
                        },
                        new() {
                            Id = "b862d453-7660-4525-ae02-fbc7e44192a0",
                            Cost = 155.28,
                            InvoiceDate = new DateTime(2025, 12, 01)
                        }
                    ]
                }
            };
        } 

        public static class MicosoftAzure
        {
            public static Payload GetExample()
            {
                return new Payload()
                {
                    Expenses = new Expenses()
                    {
                        Currency = "EUR",
                        Monthly = [
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 01, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "120.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 02, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "118.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 03, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "121.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 04, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "125.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 05, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "122.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 06, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "127.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 07, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "116.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 08, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "115.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 09, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "117.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 10, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "120.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 11, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "120.00"
                                }
                            },
                            new Monthly()
                            {
                                IssueDate = new DateTime(2025, 12, 01),
                                Bill = new Bill()
                                {
                                    Reference = 1,
                                    MoneyToPay = "124.00"
                                }
                            }
                        ]
                    }
                };
            }
        }
    }
}
