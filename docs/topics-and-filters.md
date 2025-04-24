# Topics and Filtered Subscriptions

## Topic Structure

Topics follow path notation to express hierarchy as shown below.
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
- **Wildcard Match**: `/sensor/temperature/*`, `/sensor/*/humidity`
- **Prefix Match**: `/sensor/*`


---

## TODO: More complex matching

- `/*` matches `/value/1/eulav`
- `/*/eulav` matches `/value/1/eulav`
- `/*/*` matches `/value/1/eulav`


- Does: `/*/value` match `/*/n*s/value`
- How does `#` fit in? Single char match?
- Does: `/###ic` match `/topic`, `/matic` and `/attic` etc.
- Can I use `/3#` in place of `/###` 
- Does: `/###*` or even `/3#*` work? At least 3 chars then match?
- Does: `/###*` match `/123/n*s`, `/val`, `/valu` and `val/ue` etc.