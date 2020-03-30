using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Core.Common.Interfaces;
using ShoppingCart.Core.Common.Models;
using ShoppingCart.Core.Cart.Interfaces;
using ShoppingCart.Core.Cart.Services;
using ShoppingCart.Core.Discount.Interfaces;
using ShoppingCart.Core.Discount.Models;
using ShoppingCart.Core.Delivery.Interfaces;
using ShoppingCart.Core.Delivery.Services;

namespace ShoppingCart.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceProvider serviceProvider = ResolveDependencies();

            ICart cart = serviceProvider.GetService<ICart>();
            IDeliveryCostCalculator deliveryCostCalculator = serviceProvider.GetService<IDeliveryCostCalculator>();

            Run(cart, deliveryCostCalculator);

            Console.ReadLine();
        }

        static ServiceProvider ResolveDependencies()
        {
            double costPerDelivery = 5.0;
            double costPerProduct = 1;
            double fixedCost = 2.99;

            return new ServiceCollection()
                .AddSingleton<ICart, Cart>()
                .AddTransient<IDeliveryCostCalculator, DeliveryCostCalculator>(x => new DeliveryCostCalculator(costPerDelivery, costPerProduct, fixedCost))
                .BuildServiceProvider();
        }

        static void Run(ICart cart, IDeliveryCostCalculator deliveryCostCalculator)
        {
            ICategory categoryConsumerElectronics = new Category("Consumer Electronics");
            IProduct productAppleWatch = new Product(categoryConsumerElectronics, "Apple Watch", 2000.0);
            IProduct productAppleMacbookPro = new Product(categoryConsumerElectronics, "Apple Macbook Pro", 10000.0);

            ICategory categoryToysAndGames = new Category("Toys & Games");
            IProduct productLego = new Product(categoryToysAndGames, "Lego Classic Pack", 100.0);

            cart.Add(productAppleWatch, 2);
            cart.Add(productAppleMacbookPro, 1);
            cart.Add(productLego, 10);

            ICollection<ICampaign> campaigns = new Collection<ICampaign>();
            ICollection<ICoupon> coupons = new Collection<ICoupon>();

            IRateCampaign inapplicableRateCampaign = new RateCampaign(categoryToysAndGames, "80% discount on Toys & Games category", .5, true, true, 50000.0, 20);
            IAmountCampaign lowAmountCampaign = new AmountCampaign(categoryConsumerElectronics, "100 TL discount on Toys & Games category", 100, false, false, 0.0, 0);
            IAmountCampaign bestAmountCampaign = new AmountCampaign(categoryConsumerElectronics, "1400 TL discount on Toys & Games category", 1400, false, true, 0.0, 3);
            IAmountCoupon inapplicableAmountCoupon = new AmountCoupon(5000, "5000 TL discount coupon", true, true, 50000, 50);
            IRateCoupon lowRateCoupon = new RateCoupon(.1, "10% discount coupon", false, false, 0.0, 0);
            IRateCoupon bestRateCoupon = new RateCoupon(.5, "50% discount coupon", true, true, 10000, 13);

            campaigns.Add(inapplicableRateCampaign);
            campaigns.Add(lowAmountCampaign);
            campaigns.Add(bestAmountCampaign);
            coupons.Add(inapplicableAmountCoupon);
            coupons.Add(lowRateCoupon);
            coupons.Add(bestRateCoupon);

            cart.ApplyDiscounts(campaigns, coupons);

            cart.DeliveryCost = deliveryCostCalculator.Calculate(cart);

            var response = cart.Print();

            Console.Write(response);
        }
    }
}
