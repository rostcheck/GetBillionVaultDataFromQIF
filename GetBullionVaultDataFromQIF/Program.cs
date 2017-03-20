using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QifApi;
using QifApi.Transactions;

namespace GetBullionVaultDataFromQIF
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			string fileName = Path.Combine("/Users/davidr/Documents/Software Projects/Xamarin/TrackMetal/TrackMetal/bin/Debug", "BV.qif");
			QifDom qifDom = QifDom.ImportFile(fileName);
			Save(qifDom.InvestmentTransactions.Where(s => s.AccountForTransfer == "Investment Expense:Metal Storage").ToList(),
				"BullionVault-Main-fees.txt");
		}

		public static void Save(List<InvestmentTransaction> orderList, string fileName)
		{
			StreamWriter writer = new StreamWriter(fileName, false); 
			writer.WriteLine("OrderDateTime\tOrderID\tAction\tVault\tValue\tClientTransRef\tCurrency\t"
				+ "Metal\tGoodTil\tLastModified\tPricePerKg\tOrderType\tStatus\tQuantity\tQtyFilled\tCommission\t"
				+ "Consideration\tTradeType");

			int counter = 0;
			foreach (InvestmentTransaction order in orderList)
			{
				writer.WriteLine(string.Format(
					"{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}",
					order.Date, string.Format("TF{0:00000000}", counter), "storage_fee", "any", order.TransactionAmount,
					"", "USD", ParseMetalType(order.TransactionAmount), order.Date,
					order.Date, 0, "fee", "closed", 
					0, 0, 0.0m, 0.0m, "fee"));
					//order.OrderDateTime, order.OrderID, order.Action, order.Vault, order.Value, 
					//order.ClientTransactionReference, order.Currency, order.Metal, order.GoodUntil,
					//order.LastModified, order.Limit, order.OrderType, order.ProcessingStatus, 
					//order.Quantity, order.QuantityFilled, order.TotalCommission, order.TotalConsideration,
					//order.TradeType));
			}
			writer.Close();
		}

		// Huge hack - infer metal type based on storage fee
		public static MetalTypeEnum ParseMetalType(decimal amount)
		{
			if (amount > 10.0m)
				return MetalTypeEnum.Silver;
			else
				return MetalTypeEnum.Gold;
		}
	}
}
