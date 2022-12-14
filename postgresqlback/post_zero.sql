PGDMP                         z            post    15.1    15.0                0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false                       0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false                       0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false                       1262    16534    post    DATABASE     x   CREATE DATABASE post WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';
    DROP DATABASE post;
                postgres    false            ?            1259    24780    message    TABLE     d   CREATE TABLE public.message (
    text_id integer NOT NULL,
    text text,
    created_date date
);
    DROP TABLE public.message;
       public         heap    postgres    false            ?            1259    24788    message_rubrics    TABLE     g   CREATE TABLE public.message_rubrics (
    text_id integer NOT NULL,
    rubrics_id integer NOT NULL
);
 #   DROP TABLE public.message_rubrics;
       public         heap    postgres    false            ?            1259    24779    message_text_id_seq    SEQUENCE     ?   CREATE SEQUENCE public.message_text_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.message_text_id_seq;
       public          postgres    false    217                       0    0    message_text_id_seq    SEQUENCE OWNED BY     K   ALTER SEQUENCE public.message_text_id_seq OWNED BY public.message.text_id;
          public          postgres    false    216            ?            1259    24771    rubrics    TABLE     X   CREATE TABLE public.rubrics (
    rubrics_id integer NOT NULL,
    rubrics_name text
);
    DROP TABLE public.rubrics;
       public         heap    postgres    false            ?            1259    24770    rubrics_rubrics_id_seq    SEQUENCE     ?   CREATE SEQUENCE public.rubrics_rubrics_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public.rubrics_rubrics_id_seq;
       public          postgres    false    215                       0    0    rubrics_rubrics_id_seq    SEQUENCE OWNED BY     Q   ALTER SEQUENCE public.rubrics_rubrics_id_seq OWNED BY public.rubrics.rubrics_id;
          public          postgres    false    214            o           2604    24783    message text_id    DEFAULT     r   ALTER TABLE ONLY public.message ALTER COLUMN text_id SET DEFAULT nextval('public.message_text_id_seq'::regclass);
 >   ALTER TABLE public.message ALTER COLUMN text_id DROP DEFAULT;
       public          postgres    false    217    216    217            n           2604    24774    rubrics rubrics_id    DEFAULT     x   ALTER TABLE ONLY public.rubrics ALTER COLUMN rubrics_id SET DEFAULT nextval('public.rubrics_rubrics_id_seq'::regclass);
 A   ALTER TABLE public.rubrics ALTER COLUMN rubrics_id DROP DEFAULT;
       public          postgres    false    215    214    215                      0    24780    message 
   TABLE DATA           >   COPY public.message (text_id, text, created_date) FROM stdin;
    public          postgres    false    217   O                 0    24788    message_rubrics 
   TABLE DATA           >   COPY public.message_rubrics (text_id, rubrics_id) FROM stdin;
    public          postgres    false    218   l                 0    24771    rubrics 
   TABLE DATA           ;   COPY public.rubrics (rubrics_id, rubrics_name) FROM stdin;
    public          postgres    false    215   ?                  0    0    message_text_id_seq    SEQUENCE SET     B   SELECT pg_catalog.setval('public.message_text_id_seq', 1, false);
          public          postgres    false    216                       0    0    rubrics_rubrics_id_seq    SEQUENCE SET     E   SELECT pg_catalog.setval('public.rubrics_rubrics_id_seq', 1, false);
          public          postgres    false    214            s           2606    24787    message message_pkey 
   CONSTRAINT     W   ALTER TABLE ONLY public.message
    ADD CONSTRAINT message_pkey PRIMARY KEY (text_id);
 >   ALTER TABLE ONLY public.message DROP CONSTRAINT message_pkey;
       public            postgres    false    217            q           2606    24778    rubrics rubrics_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.rubrics
    ADD CONSTRAINT rubrics_pkey PRIMARY KEY (rubrics_id);
 >   ALTER TABLE ONLY public.rubrics DROP CONSTRAINT rubrics_pkey;
       public            postgres    false    215            t           2606    24796 /   message_rubrics message_rubrics_rubrics_id_fkey    FK CONSTRAINT     ?   ALTER TABLE ONLY public.message_rubrics
    ADD CONSTRAINT message_rubrics_rubrics_id_fkey FOREIGN KEY (rubrics_id) REFERENCES public.rubrics(rubrics_id);
 Y   ALTER TABLE ONLY public.message_rubrics DROP CONSTRAINT message_rubrics_rubrics_id_fkey;
       public          postgres    false    215    3185    218            u           2606    24791 ,   message_rubrics message_rubrics_text_id_fkey    FK CONSTRAINT     ?   ALTER TABLE ONLY public.message_rubrics
    ADD CONSTRAINT message_rubrics_text_id_fkey FOREIGN KEY (text_id) REFERENCES public.message(text_id);
 V   ALTER TABLE ONLY public.message_rubrics DROP CONSTRAINT message_rubrics_text_id_fkey;
       public          postgres    false    218    3187    217                  x?????? ? ?            x?????? ? ?            x?????? ? ?     