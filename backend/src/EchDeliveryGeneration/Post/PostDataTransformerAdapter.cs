// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Ech0228_1_0;
using EchDeliveryGeneration.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration;

namespace EchDeliveryGeneration.Post;

public class PostDataTransformerAdapter
{
    private readonly IServiceProvider _sp;

    public PostDataTransformerAdapter(IServiceProvider sp)
    {
        _sp = sp;
    }

    public Delivery Transform(PostDataTransformerContext context)
    {
        context.EnsureIsValid();

        var transformer = _sp.GetRequiredKeyedService<IPostDataTransformer>(context.PostPrintVersion);
        var delivery = transformer.Transform(context);
        SetDeliveryExtension(delivery, context.JsonConfig!);
        return delivery;
    }

    private void SetDeliveryExtension(Delivery delivery, Configuration jsonConfiguration)
    {
        var deliveryExtension = new DeliveryExtension
        {
            Certificates = jsonConfiguration.Certificates,
        };

        foreach (var jsonConfigurationPrinting in jsonConfiguration.Printings)
        {
            foreach (var municipality in jsonConfigurationPrinting.Municipalities)
            {
                deliveryExtension.Municipalities.TryAdd(municipality.Bfs, municipality);
            }
        }

        delivery.VotingCardDelivery.Extension = deliveryExtension;
    }
}
