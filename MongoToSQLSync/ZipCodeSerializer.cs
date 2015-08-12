using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoToSQLSync
{
    public class ZipCodeSerializer : IBsonSerializer
    {
        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.CurrentBsonType;
            switch (bsonType)
            {
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return null;
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.Int32:
                    return context.Reader.ReadInt32().ToString();
                default:
                    var message = string.Format("ZipCodeSerializer expects to find a String or an Int32, not a {0}.", bsonType);
                    throw new BsonSerializationException(message);
            }
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString((string)value);
            }
        }

        public Type ValueType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
