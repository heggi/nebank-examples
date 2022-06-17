<?php

$host = 'https://gateway.nebank.heggi.dev';
$login = 'test1';
$privateKey = 'file://private-1.pem';

$request = [
    'paymentId' => 6,
    'orderId' => 'test-partial-1',
    'partial' => true,
    'amount' => 1000,
];

$requestJson = json_encode($request);

$pkeyid = openssl_get_privatekey($privateKey);
$res = openssl_sign($requestJson, $signature, $pkeyid, OPENSSL_ALGO_SHA256);

$auth = base64_encode($login . ':' . base64_encode($signature));

$context = stream_context_create([
    'http' => [
        'method'  => 'POST',
        'header'  => [
            'Content-Type: application/json',
            'Authorization: Basic ' . $auth,
        ],
        'content' => $requestJson
    ],
]);

$result = file_get_contents("${host}/api/acquiring/create-refund", false, $context);
echo ($result);
