// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Ech0228_1_0;

namespace EchDeliveryGeneration.Post;

public interface IPostDataTransformer
{
    Delivery Transform(PostDataTransformerContext context);
}
