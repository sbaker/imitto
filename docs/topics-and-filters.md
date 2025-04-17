# Topics and Filtered Subscriptions

## Topic Structure

Topics follow url notation to express hierarchy as shown below.
Topics must be in a reletive uri format beginning with a `/` and
must not contain any spaces or special characters. (`/user/invite/send-email`)

```
root
├── sensor
│   ├── temperature
│   │   ├── indoor
│   │   └── outdoor
│   └── humidity
└── system
    └── status
```

## Topic Filters
Topics are hierarchical and can be filtered using wildcards.
This allows for flexible subscription to messages based on their topic structure.
When subscribing to topics, you can use filters to specify which messages you want to receive.
Filters can be exact matches, wildcards, or prefixes.

- **Exact Match**: `/sensor/temperature/indoor`
- **Wildcard Match**: `/sensor.temperature/*` `/sensor/*/humidity`
- **Prefix Match**: `/sensor/*`
